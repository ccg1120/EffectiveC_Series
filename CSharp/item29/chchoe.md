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

    private class Letter 
}
```
