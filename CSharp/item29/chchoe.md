# Effective c# 

# Linq 활용

LINQ가 구현된 이유 : 지연된 쿼리를 지원하고 다양한 데이터 저장소에 대해 쿼리를 수행할 수 있는 통합 구문을 제공하기 위해

LINQ의 목표

데이터 소스의 유형과 상관없이 동일한 작업을 수행하는 코드를 손쉽게 작성하는것

쿼리와 실제 데이터 소스를 연경해주는 쿼리 제공자를 자유롭게 구현 할 수 있는 기능도 제공 

기능과 구현 내용을 이해하면 LINQ의 동작 방식을 좀더 명확하게 이해할 수 있음.

# 아이템 29: 컬랙션을 반환하기보다 이터레이터를 반환하는 것이 낫다.

시퀀스를 반환하는 메서드를 작성해야 한다면 컬렉션을 반환하기보다는 이터레이터를 반환하는 것이 좋음

다양한 적업을 좀더 수월하게 수행 할 수 있기 때문.

이터레이터 메서드란? (이건 찾아보자)  

호출자가 요청한 시퀀스를 생성하기 위해서 yield return 문을 사용하는 메서드를 말함

```c#
public static IEnumerable<char> GenerateAlphabet()
{
    var letter = 'a';
    while(letter <= 'z')
    {
        yield return letter;
        letter++;
    }
}
```
이터레이터 메서드에서 가장 흥미로운 부분은 컴파일러가 어떻게 해석하고 코드를 생성하는지에 대한 부분임

아래 코드와 유사한 클래스가 생성됨

```c#
class EmbeddedIterator : IEnumerable<char>
{
    public IEnumeratoer<char> GetEnumerator() =>
        new LetterEnumerator();
    IEnumerator Ienumerable.GetEnumerator() =>
        new LetterEnumerator();
    
    public static Ienumerable<char> GenerateAlphabet() =>
        new EmbeddedIterator();

    private class LetterEnumerator : IEnumerator<char>
    {
        private char letter = (char)('a'-1);

        public bool MoveNext()
        {
            letter++;
            return letter <= 'z';
        }
        public char Current => letter;

        object IEnumrator.Current => letter;

        public void Reset() =>
            letter = (char)('a'-1);

        void IDisposable.Dispose(){}
    }
}
```

```c#
var allNumbers = Enumerable.Range(0, int.MaxValue);
```
위 코드는 숫자 시퀀스를 만들어내는 객체를 생성함  
호출 측에서는 이터레이터 메서드의 결괏값을 추가적인 컬렉션에 저장하지 않는 이상 방대한 결과치를 저장하기 위한 공간이 필요하지 않음

필요한 객체의 수를 사전에 예측하기란 불가능함

'필요할 때 생성' 이라는 전략은 이터레이터 메서드를 작성할 때 가장 중요한 전략 중 하나임  
이터레이터 메서드는 시퀀스를 생성하는 방법을 알고 있는 객체를 생성 하고 이 객체는 실제 시퀀스에 접근이 이루어지는 경우에만 사용됨

```c#
public static IEnumerable<char> GenerateAlphabetSubset(char first, char last)
{
    if(first < 'a')
        throw new ArgumentException();
    if(first > 'z')
        throw new ArgumentException();

    if(last < first)
        throw new ArgumentException();
    if(last > 'z')
        throw new ArgumentException();
        
    var letter = first;
    
    while(letter <= last)
    {
        yield return letter;
        letter++;
    }
}
```

컴파일러는 이 코드는 다음과 유사한 코드를 생성함 

```c#
public class EmbeddedSubsetIterator : IEumerable<char>
{
    private readonly char first;
    private readonly char last;

    public EmbeddedSubsetIterator(char first, char last)
    {
        this.first = first;
        this.last = last;
    }

    public IEnumerator<char> GetEnumerator() =>
        new LetterEnumerator(first, last);
    IEnumerator IEnumerable.GetEnumerator() =>
        new LetterEnumerator(first, last);
    
    public static IEnumerable<char> GenerateAlphabetSubset(char first, char last) => 
        new EmbeddedSubsetIterator(first, last);

    private class LetterEnumerator : IEnumerator<char>
    {
        private readonly char first;
        private readonly char last;

        private bool isInitialized = false;

        public LetterEnumerator(char first, char last)
        {
            this.first = first;
            this.last = last;
        }

        private char letter = (char)('a'-1);
        public bool MoveNext()
        {
            if(isInitialized == false)
            {
                if(first < 'a')
                    throw new ArgumentException();
                if(first > 'z')
                    throw new ArgumentException();

                if(last < first)
                    throw new ArgumentException();
                if(last > 'z')
                    throw new ArgumentException();

                letter = (char)('a'-1);
                isInitialized = true;
            }
            letter++;
            return letter <= last;
        }
        public char Current => letter;
        object IEnumerator.Current => letter;
        public void Reset() => isInitialized = false;
        void IDisposable.Dispose(){}   
    }
}
```

살펴볼 부분으로 시퀀스의 첫번째 요소가 요청될 때까지 매개변수의 값이 유효한지를 확인하는 코드가 수행되지 않는다는것  
메서드에 잘못된 인자를 넘겼을 경우 문제를 확인하고 해결하기 까다로움

시퀀스를 사용하려 할 때 예외가 발생하기 때문임

코드를 약간 재구성하여 인자에 대한 유효성 검사를 수생하는 부분과 실제 시퀀스를 생성하는 부분을 분리할 수 있음

```c#
public static IEnumerable<char> GenerateAlphabetSubset(char first, char last)
{
    if(first < 'a')
        throw new ArgumentException();
    if(first > 'z')
        throw new ArgumentException();

    if(last < first)
        throw new ArgumentException();
    if(last > 'z')
        throw new ArgumentException();

    return GenerateAlphabetSubsetImpl(first, last);
}

private static IEnumerable<char> GenerateAlphabetSubsetImpl(first, last)
{
    var letter = first;
    
    while(letter <= last)
    {
        yield return letter;
        letter++;
    }
}
```

메서드에 잘못된 인자를 전달한 경우  private메서드로 진입하기 이전에 public 메서드 내에서 예외가 발생하게 됨

뒤늦게 예외가 발생하는 것이 아니라 즉각적으로 발생하여 좀더 쉽게 오류를 확인하고 수정 할 수 있음

어떤 경우에 이터레이터 메서드를 이용하여 시퀀스를 생성하는 것이 좋은지 알아보자

시퀀스를 반복적으로 사용하는 경우라면 시퀀스를 캐싱하는 것은 어떤가? 이는 사용자들이 결정할 문제임

ToList()나 ToArray()와 같은 확장 메서드를 이용하면 IEnumerable<T> 타입을 이용하여 시퀀스로부터 컬렉션을 생성 할 수 있음

계산 시간과 저장소 공간을 모두 고려한다면 간혹 시퀀스를 반환하는 메서드가 전체 시퀀스를 단번에 생성하여 반환하는 메서드가 비용이 더 클 수도 있음. 하지만 사용자들이 어떻게 사용할 지 예측 할 수 없음으로 과도하게 신경쓰기보다 API를 쉽게 사용할 수 있도록 배려하는것에 집중 하자