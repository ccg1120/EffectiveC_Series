# Effective c# 


# 아이템 33: 필요한 시점에 필요한 요소를 생성하라

이터레이터 메서드가 입력 매개변수로 반드시 시퀀스를 전달받아야 할 필요는 없음.

새로운 요소를 생성하는 팩토리 메서드로 사용 할 수 있음.

이 방법으로 코드를 작성하면 사용되지 않을 요소를 미리 생성 하는 것을 피할 수 있음.

```c#
static IList<int> CreateSequence(int numberOfElements, int startAt, int stepBy)
{
    var collection = new List<int>(numberOfElements);

    for(int i=0; i < numberOfElements; i++)    
        collection.Add(startAt + i * stepBy);

    return collection;
}
```

위 예제는 yield return을 이용하여 생성하는 것보다 단점이 많음.

클라이언트가 BindingList\<int>와 같이 다른 타입을 요구하면 변환 작업을 반드시 수행해야함
```c#
var data = new BindingList<int>(CreateSequence(100,0,5).ToList());
```

위 코드는 미묘한 버그가 발생 할 수 있음

BindingList\<T>는 리스트를 복사하지 않고 동일한 메모리 공간을 그대로 재사용하는 특징이 있음  
매개변수로 전달한 객체를 다른곳에서 쓴다면 일관성 문제가 발생 할 수 있음

중간에 작업을 중단 할 수 없음 
CreateSequence() 메서드는 항상 요청된 개수만큼 요소를 생성, 사용자가 페이징이나 다른 이유로 작업으로 중단하고 싶어도 방법이 없음

시퀀스를 생성하는 기능을 이터레이터 메서드로 만들면 문제를 모두 피할 수 있음 

```c#
static IEnumerable<int> CreateSequence(int numberOfElements, int startAt, int stepBy)
{
    for(var i=0; i<numberOfElements; ++i )
    {
        yield return startAt + i * stepBy; // 주목!
    }
}
```

위 코드는 이전 코드와 핵심 코드는 거의 동일함  
새롭게 작성한 코드는 이전 코드와 실행 방식에 차이가 있음을 주목 

시퀀스 내의 개별 요소가 요청 시마다 하나씩 생성됨

따라서 동일한 int 타입의 요소를 생성하기 때문에 작업의 내용이 변경 되지 않음 

생성된 시퀀스를 List\<int> 에 저장하고 싶다면 IEnumerable\<int>를 매개변수로 취하는 생성자를 사용하면 됨

```c#
var listStorage = new List<int>(CreateSequence(100, 0, 5));
```

다음 코드를 작성하면 BindList\<int> (double은 오타겠지...?) 컬랙션을 생성 할 수도 있음

```c#
var data = new BindingList<int>(CreateSequence(100, 0, 5).ToList());
```

코드가 비효율적으로 보임. 동일 시퀀스가 두번 생길거 같지만 BindingList가 복사를 하지 않기 때문에 비효율적이진 않음

코드를 잘 작성하면 시퀀스에 대한 순회 과정을 쉽게 중단 할 수도 있음.

조건에 부합하지 않는 상황이 됐을 때 추가로 숫자를 생성하지 않으므로 성능상에 이점이 있음

```c#
// 익명 델리게이트 사용
var sequence = CreateSequence(10000, 0, 7).TakeWhile(delegate (int num){return num < 1000});
// 람다 표기법 사용
var sequence = CreateSequence(10000, 0, 7).TakeWhile((num) => num<1000);
```

위 예제를 통해 이터레이터 메서드를 사용하면 어느 때나 간단히 순회를 중단 할 수 있음.

어떠한 경우라도 필요한 시점에 맞춰 필요한 요소를 생성하도록 코딩하는 것이 가장 효과적임