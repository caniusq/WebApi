Cus.WebApi
==========

* A simple .net c# WebApi Framework without mvc.
* Auto generate documentaion for your api.

##Usage:

in Global.asax:
```cs
using Cus.WebApi;

protected void Application_Start(object sender, EventArgs e)
{
	RouteTable.Routes.MapWebApiRoute("DefaultApi");
}
```

in your api class:
```cs
/// <summary>
/// test's documentation
/// </summary>
[Documentation("~/App_Data/Cus.WebApi.Test.XML")]
public class test : ApiController
{
	/// <summary>
	/// Foo's documentation
	/// </summary>
	/// <param name="bar">bar's documentation</param>
	/// <returns>return value</returns>
	public string Foo(string bar)
	{
		return "bar";
	}
}
```

then open url:

http://www.yourwebsite.com/api
