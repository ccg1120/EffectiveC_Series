# Effective c# 


# 아이템 41: 값비싼 리소스를 갭처하지 말라


클로저는 클로저에 바인딩된 변수를 초함하는 객체를 생성함.  
바인딩 된 변수의 수명이 길어지면 문제를 일으킬 수 있으므로 주의가 필요함

블록을 벗어나면 가비지 수집기에 의해 정리 되는것이 보통임, 하지만 클로저와 캡쳐된 변수는 이러한 규칙을 벗어남

일반적인 경우에는 신경 쓸 필요가 없음, 왜? 적절한 시점에 가비지 수집이 될 것이기 때문


```c#
var counter = 0;
var numbers = Extensions.Generate(30, ()=>counter++);
```

위 코드는 다음과 같이 생성됨

```c#
{
    private class Closure
    {
        public int generatedCounter = 0;
        public int generatorFunc() => generatedCounter++;
    }

    var c = new Closure();
    c.generatedCounter = 0;
    var sequence = Extensions.Generate(30, new Func<int>(c.generatorFunc));
}
```

내부적으로 숨겨진 중첩 클래스가 정의 됨
델리게이트는 숨겨진 클래스 타입의 객체 수명에 영향을 주며 언제 가비지화 될지에 대해서도 영향을 미침


```c#
public IEnumerable<int> MakeSequence()
{
    var counter = 0;
    var numbers = Extensions.Generate(30, ()=>counter++);
    return numbers;    
}
```

반환 객체는 클로저에 결합된 델리게이트를 사용함, 반환 객체가 델리게이트를 필요로 하므로 수명도 연장되고 변수를 포함하는 객체의 수명도 늘어나게 됨

위 코드는 아래 코드와 같은 코드를 생성함
```c#
public static IEnumerable<int> MakeSequence()
{
    var  c = new Closure();
    c.generatedCounter = 0;
    var sequence = Extensions.Generate(30, new Func<int>(c.generatorFunc));
    return sequence;    
}
```

클로저는 지역변수 이지만 메서드를 벗어나도 살아남게됨, 대부분의 경우 큰 문제가 되는 경우가 없음

IDisposable이 연관되어 있는 경우 문제가 발생함


```c#
public static IEnumerable<string> ReadLine(this TextReader reder)
{
    var txt = reader.ReadLine();
    while(txt != null)
    {
        yield return txt;
        txt = reader.ReadLine();
    }
}

public static int DefaultParse(this string input, int defaultValue)
{
    int answer;    
    return (int.TryParse(input, out answer)) ? answer : defaultValue;
}

public static IEnumerable<IEnumerable<int>> ReadNumbersFromStream(TextReader t)
{
    var allLines = from line in t.ReadLine()
                    select line.Split(',');

    var matrixOfValues = from Line in allLines
                        select from item in line 
                        select item.defaultParse(0);
    
    return matrixOfValues;
}

{
    var t = new StreamReader(File.OpenRead("TestFile.txt"));
    var rowOfNumbers = ReadNumbersFromStream(t);
}
```

ReadNumbersFromStream은 모든 결괏값을 메모리에 저장하는 것이 아니라 필요한 시점에 값을 로드함. 
실제로 파일을 바로 읽지 않으며 rowOfNumbers를 순회하는 코드가수행 될 때 파일을 열고 읽기 시작함

여러 문제가 있는 코드임

```c#
IEnumerable<IEnumerable<int>> rowOfNumbers;
using (TextReader t = new StreamReader(File.OpenRead("TestFile.txt")))
    rowOfNumbers = ReadNumbersFromStream(t);
```

위 처럼 수정 해도 해결은 안됨

결과적으로 수정하기 위해선 방법은 단순함, 파일을 닫기 전에 값에 대한 순회를 마치면 됨

```c#
using (TextReader t = new StreamReader(File.OpenRead("TestFile.txt")))
{
    var arrayOfNums = ReadNumbersFromStream(t);
    foreach(var line in arrayOfNums)
    {
        foreach(var num in line )
        {
            write("{0},", num);
            writeLine(e();
        }
    }
}
```
위 코들를 작성 하면 실제로 파일을 어느 위치에서 닫아야 할지 알수 가 없음

값을 순회하는 부분이 API로 구성 되어 있다면 파일이 열려있어야 함

```c#
using (TextReader t = new StreamReader(File.OpenRead("TestFile.txt")))
    return ReadNumbersFromStream(t); // api로 순회하는 코드라면?
```

파일을 온전히 닫을 방법이 없음

한가지 확실한 해결책은 파일을 열고 시퀀스를 모두 읽은 후 그 시퀀스를 반환하도록 메서드를 변경 하는것

```c#
public static IEnumerable<string>ParseFile(string path)
{
    using(var r = new StreamReader(File.OpenRead(path))) // 이 함수가 끝나면 여기서 파일이 알아서 닫힘
    {
        var line = r.ReadLine();
        while(line != null)
        {
            yield return line;
            line = r.ReadLine();
        }
    }
}
```


간단 예시 코드
```c#
class Generator : IDisPosable
{
    private int count;
    public int GetNextNumber() => count++;
    public void Dispose()
    {
        WriteLine("Disposing now...");
    }
}
```

 IDisposable을 구현하고 있는 타입의 인스턴스를 캡처 했을 때를 설명 하기 위한 코드임

 ```c#
public IEnumrable<int> SomeFunction()
{
    using (Generator g = new Generator())
    {
        while(true)
            yield return g.GetNextNumber();
    }
}

var query = (from n in SomeFunction() select n).Take(5);

foreach(var s in query )
    Console.WriteLine(s);

WriteLine("again");

foreach(var s in query )
    Console.WriteLine(s);

// "Disposing now..." 이 두번 호출되는 것을 볼 수 있음
 ```


파일을 다루는 경우였다면 두 번째 순회에서 예외가 발생함

IDisposable 인터페이스를 구현한 리소스에 대해 반복적으로 순회해야 한다면 다른 처리 방법이 필요함

값을 읽고 처리한느 루틴에 델리게이트를 이용하여 서로 다른 로직을 전달 할 수 있도록 코드를 작성하는 것이 현명한 방법임


```c#
private static IEnumerable<int> LeakingClosure(int mod)
{
    var filter = new ResourceHogFilter();
    var source = new CheapNumberGenerator();
    var results = new CheapNumberGenerator();

    var importantStatistic = (from num in source.GetNumbers(50) where filter.PassesFilter(num) select num).Average();
    
    return from num in results.GetNumbers(100) where num > importantStatistic select num;
}
```

c# 컴파일러는 클로저를 구현할 때 메서드 내에서 단 하나의 중첩 클래스만 생성함

저중 하나가 값비싼 리소스 라면 범위를 벗어나 문제가 될 수 있음 

문제 해결 방법은 메섣드를 두 부분으로 분리하여 컴파일러가 2개의 독립된 중첩 클래스를 생성 하도록 하면됨

```c#
private static IEnumerable<int> NotLeakingClosure(int mod)
{
    var importantStaticstic = GenerateImportantStatistic();
    var results = new CheapNumberGenerator();
    return from num in results.GetNumbers(100) where num > importantStaticstic select num;
}

private static double GenerateImportantStatistic()
{
    var filter = new ResourceHogFilter();
    var source = new CheapNumberGenerator();
    return (from num in source.GetNumbers(50) where filter.PassesFilter(num) select num).Average();
}
```

다른걸 못느낄 수 있지만 Average는 전체 시퀀스를 필요로 하기 때문에 ResourceHogFilter 객체를 포함하는 클로저는 반환되면서 가비지가 됨

어느 경우든 클로저에 의해 생성된 객체를 메서드가 반환하는 경우 클로저를 수행하기 위해 캡처됐던 모든 변수들이 그 안에 포함된다는 사실을 알아야 하고 면밀히 살펴야 하고 클로저가 변수를 제대로 정리 하는지 확인이 필요함



