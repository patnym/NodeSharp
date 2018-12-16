
# What is this?

NodeSharp is a libarary to bind C# interfaces to Javascript functions running inside a Node process, allowing you to call Javascript functions from your C# program.

# Why?

My main work is within C# .NET core, but I always find myself doing proof of concecpts within NodeJS due to the size of NPM's repository and community packages. 

I created NodeSharp because I wanna be able to integrate these packages into my existing application when creating POC's. 
This saves me from having to demonstrate an entire new application and lets me show off a better POC.

# NOTES!

- THIS IS PROJECT IS ON-GOING.
    - Only my demo version works, If you wanna add your own impl you need to look into the **NodeSharpBinder Bind()** method, where I currently "hardcode" the connection between Javascript and C#.
    - Only supports integer parameters and return types
    - Only works on Windows, and only tested on **Windows 10 Version	10.0.17134 Build 17134**

- I intend to extract the Javascript library from this repository make it stand-alone.

# Example

**Coming soon!**

### Preview example (How I intend this to work!)

**MyJavascriptFunctions.js**
```javascript
const ns = require('ns')

function add(x, y) {
    return x + y
}

//Bind this function to NodeSharp, define param types and return types
ns.bind(add, (nsf) => {
    nsf.arguements = [ new NsArguement('int'), new NsArguement('int') ];
    nsf.returns = new NsArguement('int');
});

``` 

**Program.cs**
```csharp

public interface MyJsFunctions 
{
    int add(int x, int y);
}

public static void Main(string[] args) 
{
    var nodeSharpBinder = new NodeSharpBinderBuilder()
        .Bind<MyJsFunctions>("<path_to_MyJavascriptFunctions.js>")
        .Build();
    var jsInCsharpWtf = nodeSharpBinder.Resolve<MyJsFunctions>();
    
    //result = 10
    var result = jsInCsharpWtf.add(10, 5);
}

```
