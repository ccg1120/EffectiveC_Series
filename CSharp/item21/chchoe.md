# Effective c# 

# 아이템 21: 타입 매개변수가 IDisposable을 구현한 경우를 대비하여 제네릭 클래스를 작성하라

제약 조건은 두가지 역할

1. 런타입 오류가 발생할 가능성이 있는 부분을 컴파일 타임 오류로 대체 해줌
2. 타입을 명확히 규정하여 사용자에게 도움을 줌 

하지만 제약조건은 무엇을 해서 안되는지를 정의할 수 없음 

타입 매개 변수로 지정한 타입이 IDisposable을 구현하고 있다면 특별한 추가 작업이 반드시 필요함 (아이템 17: 표준 Dispose패턴을 구현하라)

문제는 제네릭 메서드 내에서 타입 매겨변수로 주어지는 타입을 이용하여 인스턴스를  생성할 경우 발생함

```c#
public interface IEngine
{
    void DoWork();
}

public class EngineDriverOne<T> where T: IEngine, new()
{
    public void GetThingsDone()
    {
        T driver = new T();//삐용삐용
        driver.DoWork();
    }
}
```

T가 IDisposable을 구현한 타입일 경우 리소스 누수가 발생할 수 있음. 

지역변수를 생성할 때마다 T가 IDisPosable을 구현하고 있는지 확인 해야하며 구현되어 있다면 다른 추가적 처리가 필요함

```c#
public interface IEngine
{
    void DoWork();
}

public class EngineDriverOne<T> where T: IEngine, new()
{
    public void GetThingsDone()
    {
        T driver = new T();//삐용삐용
        using(direver as IDisposable) //추가적 처리 
        {
            driver.DoWork();
        }
    }
}
```

using문에서 형변환을 사용함  
컴파일러가 IDisposable로 형변환된 객체를 저장하기 위해 숨겨진 지역변수를 생성  
T가 IDisposable을 구현하지 않았다면 지역변수의 값은 null이 됨, null인지 검사한 후 Dispose()를 호출하도록 코드를 생성함

- using을 사용하면 IL이 try/Finally 코드로 변환하고 Dispose()를 호출 하는 코드가 작성된다고 함.  
문서 참고 [MS-IDisposable](https://learn.microsoft.com/ko-kr/dotnet/api/system.idisposable?view=net-7.0)  

매우 단순한 관용 패턴임  
인스턴스를 생성한다면 반드시 앞에서와 같이 using문을 사용해야함

타입 매개변수로 전달한 타입을 이용하여 멤버 변수를 선언한 경우에는 이보다 조금 더 복잡함  
제테릭 클래스에서 IDisposable을 구현하여 해당 리소스를 처리 해야함

```c#
public sealed class EngineDriverOne<T> : IDisposable where T: IEngine, new()
{
    // 생성 작업이 오래 걸릴 수도 있으므로, Lazy를 이용하여 초기화
    private Lazy<T> driver = new Lazy<T>(() => new T()); //타입 매개변수로 멤버 변수 선언

    public void GetThingsDone() => driver.Value.DoWork();

    public void Dispose() //IDisposable 구현하여 직접 처리
    {
        if(driver.IsValueCreated)
        {
            var resrouce = driver.Value as IDisposable;
            resource?.Dispose();
        }
    }
}
```

1. IDisposable 인터페이스를 구현
2. sealed를 추가하여 파생클래스 생성을 못하도록 함
    - 파생 클래스를 만들 가능성이 있다면 패턴 전체 구현해야함 (아이템17 참조)
3. Dispose() 메서드를 한번만 호출한다고 보장하지 않음
    - 여러번 호출하는 경우에도 문제가 없도록 구현해야함
    - T에 대한 제약 조건으로 class를 설정하지 않았기 때문에 driver를 null로 설정할 수 없음(값 타입은 null로 설정 할 수 없음)


인터페이스를 조금 변경 하면 복잡한 설계를 피할 수 있음

Dispose의 호출 책임을 제네릭 클래스 외부로 전담 시키고 객체의 소유권을 외부로 옮기면 new() 제약 조건을 제거 할 수 있음

```c#
public sealed class EngineDriver<T> where T : IEngine
{
    private T driver;

    public EngineDriver(T driver)
    {
        this.driver = driver; // 제네릭 클래스가 소유권을 가지고 있지 않음
    }

    public void GetthingDone()
    {
        driver.DoWork();
    }
}
```
문제의 해결 방법은 응용프로그램을 어떻게 설계하느냐에 달렸음

제네릭 클래스의 타입 매개변수로 개체를 생성하는 경우 이타입이 IDisposable을 구현하고 확인 해야함  
항상 방어적인 코드를 작성 할 것, 리소스 누수가 되지 않도록 주의 하자

타입 매개변수로 객체를 생성하지 않도록 응용 프로그램의 구조를 변경할 수도 있음.  

타입 매개변수로는 지역변수 정도만 생성하도록 코도를 작성하는 것이 좋음.

-------------
마지막으로 타입 매개변수로 멤버를 변수로 선언해야 하는 경우라면 지연 생성을 사용해야 할 수도 있고  
IDisposable을 구현해야 할 수도 있음 작업이 필요하지만 유용한 클래스를 작성하려면 불가피한 작업임




 

 