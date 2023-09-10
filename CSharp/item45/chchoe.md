# Effective c# 


# 예외 처리

오류는 항상 발생함.

예외가 발행 했을 때 어떻게 처리해야 할지 정확히 이해하는 것은 C# 개발자의 핵심 역량 중 하나임

.NET Framework의 설계 지침에 따르면 올바르게 수행할 수 없다면 예외를 발생 시키라고 가이드하고 있음

[C# 예외에 대한 모범 사례](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)

# 아이템 45: 메서드가 실패했음을 알리기 위해서 예외를 이용하라

메서드가 작업을 제대로 수행할 수 없는 경우 예외를 발생시켜 실패가 발생했음을 알려야함

오류 코드를 이용한 처리할 경우
- 사용자가 쉽게 무시 할수 있음
- 오류 코드 처리 때문에 실행 흐름을 혼란 스럽게 함

  
예외는 일반적인 실행 흐름을 제어하는 메커니즘으로 사용 하면 안됨

예외가 발생할 가능성을 최소화하는 것이 좋음  왜냐?
- 상당히 값비쌈
- 대응하는 코드를 작성 하는 것이 쉽지 않음


반환 코드를 이용하여 오류를 보고하는 방식 보다 예외를 이용하는 방식이 장점이 더 많음

1. 오류에 대한 추가적인 정보를 전달 할 수 있음  
예외는 콜 스택을 통해 적절한 catch문이 구성된 위치까지 콜스택을 통해 전파 됨  
예외 클래스를 사용하면 에러에 대한 세부정보를 잃지 않을 수 있음

2. 예외는 쉽게 무시하기 어려움  
적절한 catch문이 포함되어 있지 않으면 응용프로그램은 종료됨  
무시한 다면 정상적인 수행이 어려움


예외를 처리하는 작업은 일반적인 메서드 호출보다 더 시간이 많이 걸림  
작업을 수행하기 전에 실패할 가능성이 있는지 확인 하는 메서드를 추가호 작성 하면 좋음

항상 예외를 유발할 조건을 사전에 검사할 수 있는 메서드를 함께 작성할 것을 권장 함 

예시1) File.Open() 전에 File.Exists()를 검사 해봄

예시2) 특정 위젯이 존재하지 않으면 완료 할 수 없는 클래스가 있다고 가정한 예시

```c#
//이런 방법은 사용 하지 말자
DoesWorkThatMightFail = worker = new DDoesWorkThatMightFail();
try
{
    worker.DoWork();
}
catch(WorkerException e)
{
    ReportErrorToUser("Test Conditions failed. Please check widgets");
}
```


```c#
//예외 발생을 최소화하는 패턴 
public class DoesWorkThatMightFail
{
    public bool TryDoWork()
    {
        if(TestConditions() == false) // 미리 확인 작업 수행
            return false;
                
        Work();
        return true;
    }

    public void DoWork()
    {
        Work(); //예외를 발생 시킬 수 있음
    }

    private bool TestConditions()
    {
        //테스트 조건 확인
        return true;
    }

    private void Work()
    {
        //작업 수행
    }
}

if(worker.TryDoWork() == false)
{
    ReportErrorToUser("Test Conditions failed. Please check widgets");
}
```

위 방식의 패턴은 예외를 유발할 가능성이 있는 작업을 수행할 때 성능을 개선하기 위한 목적으로 .NET 내부에서도 사용됨.  
예외가 발생할 가능성이 있는 메서드를 호출하기 이전에 사전에 조건을 테스트 해볼 수 있고 예외로 인한 성능 저하 문제를 피할 수 있음

특정 메서드가 작업을 온전히 완료 할 수 없을 경우 예외를 발생시킬지 결정 하는 것은 개발자의 몫임  
오류가 발생하면 항상 예외를 발생시키도록 코드를 작성 하는 것이 좋음  

예외는 일반적인 흐름 제어 메커니즘으로 사용해서는 안됨. 사전에 테스트 할 수 있는 메서드를 같이 제공하는것이 좋음