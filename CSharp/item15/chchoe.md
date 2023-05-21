# Effective c# 

# 아이템 15: 불필요한 객체를 만들지 말라

가비지 수집기는 사용자를 대신하여 메모리를 관리하며 객체를 효율적인 방식으로 제거 함  

하지만 너무 많은 객체를 생성하면 심각한 성능 문제를 일으킬 수 있음!

따라서 가비지 수집기가 과도하게 동작하지 않도록 주의 해야함

-------------------

모든 참조 타입의 객체는 동적으로 메모리를 할당함

객체를 참조하는 상위 객체가 삭제되면 가비지가 됨

지역변수의 경우 선언된 메서드를 벗어나는 순간 가비지가 됨

나쁜 예 중 윈도우 Paint 이벤트 핸들러 내에서 GDI객체를 할당하는 경우 임

```c#

protected override void OnPaint(PaintEventArgs e)
{
    usint (Font MyFont = new Font("Arial", 10.0f)) // 매번 Font객체를 생성 
    {
        e.Graphiccs.DrawString(DateTime.Now.ToString(),
        MyFont, Brushes.Black, new Pointf(0,0));
    }
    base.OnPaint(e);
    //메서드 영역을 벗어나면 가비지가 됨
}
```

OnPaint()는 매우 자주 호출되는 이벤트 핸들러 중 하나인데 호출될 때 마다 동일한 Font객체를 매번 생성 해주고 있음  
따라서 가비지 수집 작업이 많이 일어날 수 있음!

가비지 수집 작업은 언제 일어 나는가??  

- 수집 작업의 주기는 사용중인 메모리 양과 할당 주기를 기반으로 결정됨  
- 메모리 할당을 자주 하면 가비지 수집이 수행될 가능성이 높음

따라서 위의 코드는 상당히 비효율적

Font객체를 멤버 변수로 변경하여 폰트 객체를  한 번만 생성한 후 이를 재사용 하는 방향으로 개선 할 수 있음

```c#
//멤버 변수로 한번만 객체 생성 후 사용
private Font _myFont = new Font("Arial", 10.0f);

protected override void OnPaint(PaintEventArgs e)
{
    e.Graphiccs.DrawString(DateTime.Now.ToString(),
    _myFont, Brushes.Black, new Pointf(0,0));    

    base.OnPaint(e);
}
```

개선된 코드는 새로운 가비지가 생성되지 않으므르 가비지 수집가가 해야 하는 일도 줄게 됨

주의사항! 

    Font타입과 같이 IDisposable 인터페이스를 구현한 타입의 객체를 지역변수에서 맴버변수로 변경하게 되면 반드시 IDisposable을 구현해야함! 
    (참조 : 아이템 17: 표준 Dispose 패턴을 구현하라)


호출빈도가 많은 경우가 아니라면 굳이 변경 할 필요는 없음

-----------------------

Brushes.Black과 같은 정적 속성은 유사한 객체를 반복적으로 할당하는 것을 피하는 또 다른 기법을 보여줌  
자주 사용되는 참조 타입의 인스턴스를 정적 멤버 변수로 선언하는 방법을 사용

지역변수를 멤버변수로 변경하면 상당히 도움이 되겠지만 충분하지 않음

프로그램이 수행 되는 동안 수십 개의 창과 컨트롤들이 생성되고 수십 개의 검정 브러시를 생성 할 것이기 때문

이러한 문제를 예상 하고 재사용 할 수 있는 검정 브러시를 만들었음 

Brushes 클래스는 내부적으로 **지연 평가(Lazy Evaluation) 알고리즘**을 사용함  
최초로 요청될 때 비로소 필요한 객체를 생성

```c#
private static Brush blackBrush;
public static Brush Black
{
    get
    {
        //검정색 브러쉬를 요청했을 때 비로소 객체를 생성함 지연 평가!
        if(blackBrush == null) 
            blackBrush = new SolidBrush(Color.Black);

        return blackBrush;
    }
}
```

응용프로그램에서 라임색 브러시를 요청 하지 않는다면 해당 브러시는 생성되지 않음


이런 기법은 개발자가 자신의 프로그램을 개발할 때 적용해 볼만 함


부정적 측면도 있음

생성된 객체가 메모리상에 필요 이상으로 오랫동안 남아 있을 수 있음.  
Dispose() 메서드를 호출해야 할 시점을 결정 할 수 없기 때문에 비관리 리소스를 삭제 할 수 없다는 것도 매우 큰 단점

-----------------------

마지막으로 변경 불가능한 타입과 관련된 부분

대표적인 예 System.String으로 string 객체가 생성 되면 내용은 수정이 불가능 함, 프로그래밍을 하다보면 string객체 내의 문자열을 변경할 수 있는 것처럼 보이기도 함.  
하지만 새로운 문자열을 가진 새로운 string객체가 생성되는 것 이전 문자열을 가지고 있던 객체는 가비지가 됨

```c#
public static void Main(string[] args)
{
    // 이 코드는 상당히 비효율적인 작업을 하는 코드임
    string msg = "Hello, ";
    msg += thisUser.Name;
    msg += ". Today is ";
    msg += System.DateTime.Now.ToString();
}
```

```c#
public static void Main(string[] args)
{
    //설명을 위한 코드, 유효한 코드는 아님
    string msg = "Hello, ";
    string tmp1 = new string(msg + thisUser.Name);
    msg = tmp1; // "Hello, ";는 가비지 ㅂㅂ
    string tmp2 = new string(msg + ". Today is ");
    msg = tmp2; // new string(msg + thisUser.Name);도 가비지 ㅂㅂ
    string tmp3 = new string(msg + System.DateTime.Now.ToString());
    msg = tmp3; // new string(msg + ". Today is "); 도 가비지 ㅂㅂ
}
```
string의 += 연산자는 완전히 새로운 string객체를 생성하여 반환함   
이보다 복잡한 경우 StringBuilder 클래스를 사용해야함

변경 불가능한 타입을 작성하는 경우 StringBuilder와 같은 기능을 함께 제공하는 것을 고려해보기 바람

----------------------------
## 정리

가비지 수집기는 응용프로그램이 사용하는 메모리를 효율적으로 관리 함 

힙에서 객체를 생헝하고 삭제하려면 여전히 시간이 필요하기 때문에 불필요한 객체를 생성하지 말아야함

### 지금 까지 객체 생성을 최소화 하기 위한 세가지 기법을 배웠음

1. 자주 사용되는 지역변수를 멤버 변수로 변경 하는 것
2. 종족성 삽입(Dependency Injection)을 활용하여 자주 사용되는 객체를 생성 했다가 이를 재활용하는 것
3. 변경 불가능한 타입으로 만드는것

효율적 코딩을 합시당