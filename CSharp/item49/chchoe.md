# Effective c# 

# 아이템 49: catch 후 예외를 다시 발생시키는 것보다 예외 필터가 낫다.

어떤 경우 예외를 잡고 처리할 것인지를 결정하는 방법으로 예외 필터를 사용하면 사후 분석을 더 효과적으로 수행할 수 있고 런타임 비용 또한 최소화 할 수 있음.

```C#
var retryCount = 0;
var dataString = default(String)

while(dataString == null)
{
    try
    {
        dataString = MakeWebRequest();
    }
    catch( TimeoutException e ) when (retryCount++ < 3) // 예외 필터
    {
        WriteLine("Operation timed out. Trying again");
        Task.Delay(1000* retryCount);
    }
}
```
컴파일러는 스택 되감기를 수행하기 이전에 예외 필터를 수행하도록 코드를 생섬함.  
예외 필터가 false를 반환하면 런타임은 콜 스택을 따라 올라가면서 앞서 발생한 예외의 타입에 부합한 catch문을 계속 찾아감.

이 과정에서 응용 프로그램의 상태는 변경되지 않으며 그대로 유지됨


```C#
var retryCount = 0;
var dataString = default(String)

while(dataString == null)
{
    try
    {
        dataString = MakeWebRequest();
    }
    catch( TimeoutException e )
    {
        if(retryCoun++ < 3)
        {
            WriteLine("Operation timed out. Trying again");
            Task.Delay(1000* retryCount);
        }
        else
            throw;
        
    }
}
```

위 두 코드는 상당히 다른 코드임

런타임은 예외를 처리할 수 있는 catch문을 찾은 후 즉각 스택 되감기를 수행함  
따라서 예외를 일으킨 메서드 내에서 선언된 지역변수의 대부분은 도달 불가능 객체가 됨.  
또 명시적으로 새로운 예외 객체를 생성한 후 이 예외를 발생시키면 예외 발생 위치가 바뀌므로 그렇게 해서는 안됨.

위 두 가지 방식은 분석과 디버깅을 수행할 때 매우 다른 양상을 나타냄.

```c#
static void TreeOfErrors()
{
    try
    {
        SingleBadThing();
    }
    catch( RecoverableException e )
    {
        throw; //이 위치가 호출 스택에 나타남
    }
}

static void TreeOfErrorsTwo()
{
    try
    {
        SingleBadThing(); //이 위치가 호출 스택에 나타남
    }
    catch( RecoverableException e ) when(false)
    {
        WriteLine("Can't happen");
    }
}
```

catch문 내에서 throw를 호출하면 try블록 내에 포함된 오류 발생 메서드에 대한 함수 호출 정보는 모두 소실됨

하지만 예외 필터를사용하면 예외를 발생시켰던 메서드의 호출 정보를 스택에 온전히 보전 할 수 있음.

예외 필터를 사용하는 편이 응용 프로그램의 성능에도 긍정적임.

catch문이 있더라도 catch문 내부로 진입할 가능성이 없는경우 최적화를 수행하여 성능을 개선함.

스택 되감기 작업이나 catch문으로의 진입 과정은 수행 성능에 상당한 영향을 미침. 진입 자체가 제한적으로이뤄지기 때문에 성능이 개선 되는 것임. 최소한 성능이 나빠지지는 않음.


