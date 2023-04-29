# Effective c# 

# 아이템 6: nameof() 연산자를 적극 활용하라

분산 시스템이 점차 대중화 됨에 따라 다른 시스템 사이에 데이터를 주고 받을 일이 많아짐  

데이터를 주고 받기 위해 **이름이나 문자열 식별자에 의존하는 간단한 라이브러리**들이 많이 사용됨  
위 방식은 간편하지만 나중에 추가 비용이 발생 할 수 있음  

-----------------
대표적인 단점 **타입 정보 손실**

타입 정보 손실에 따른 문제점

- 타입 정보를 활용하여 추가 기능을 제공하는  개발 도구의 도움을 받지 못함
- 정적 타입 언어의 주요 장점을 상실

c# 6.0 nameof() 라는 연산자가 추가됨  
역할은 심볼 그 자체를 해당 심볼을 포함하는 문자열로 대체 해줌

```c#
public string Name
{
    get
    {
        return name;
    }
    set
    {
        if(value != name)
        {
            name = value;
            PropertyChanged?.Invoke(
                this, 
                new PropertyChangedEventArgs( nameof(Name) )); //여기 주목
        }
    }
}
private string name;
```

nameof() 연산자를 사용 했기 때문에 속성의 이름이 변경할 경우 이벤트의 인자로 전달 해야 하는 문자열도 쉽게 변경 가능 

nameof()는 심볼의 이름을 평가, 타입, 변수, 인터페이스, 네임스페이스 에도 사용 가능
(예시 코드 넣을 것)

-----------------

제네릭 타입 사용 시 부분 제약 사항이 있음!  
- 모든 타입 매개변수를 지정한 닫힌 제네릭 타입만 사용 가능

**열린 제니릭 타입이란?**

아래 코드 처럼 타입이 정해지지 않은 상태
```c#
class Item<TKey, TValue>
{
    public Item(TKey key, TValue value)
    {
        _key = key;
        _value = value;
    }

    private TKey _key;
    private TValue _value;
}
```

닫힌 제네릭 타입이란?




nameof() 연산자는 항상 로컬 이름을 문자열로 반환 (로컬 문자열??)  
완전 정규화된 이름(예: System.Int.MaxValue)을 사용하더라도 항상 로컬 이름(MaxValue)을 반환함

기본적인 활용 예는??


또 다른 활용 예시

예외 타입은 매개변수의 이름 자체를 생성자의 매개변수로 취함

```c#
public static void ExceptionMessage(object thisCantBeNull)
{
    if(thisCantBeNull == null)
    {
        throw new ArgumentNullException(
            nameof(thisCantBeNull), 
            "We told you this cant be null"
        );
    }
}
```

특성의 매개변수로 문자열을 전달해야 하는 경우에도 nameof() 연산자를 (위치 인자, 명명된 인자 모두에 대해서) 사용 할 수 있음  

경로 이름으로 네임스페이스 이름을 사용하는 것을 고려해 보는것도 좋음  

nameof() 연산자를 사용하면 심볼의 이름을 완전히 바꾸거나 수정할 경우에도 쉽게 변경사항을 반영 할 수 있음.  

가능한 한(문자화되지 않은 형태) 심볼을 유지할 수만 있다면 자동화 도구를 활용 할 수 있는 가능성이 높아짐  
그로 인해 다른 도구를 사용하거나 개발자가 직업 내용을 눈으로 검토하는 것에 비해 오류 발견이 쉬움, 더 어려운 문제에 역량 집중 가능


















