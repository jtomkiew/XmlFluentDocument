### Build

![BuildStatus](https://jtomkiew.visualstudio.com/_apis/public/build/definitions/c3a75e86-a502-4d07-b951-de6061a50719/1/badge)

### Example

```cs
var fXml = XmlFluent.XmlFluentDocument.New()
	.XmlDeclaration() // optional
	.RootNode("elements").In(r => r // root element
		.Node("element").In(n => n // node with children
			.Node("data1", a => a // node with attributes and inner text
				.Attribute("name", "DATA 1 NAME") // attribute
				.Attribute("value", "DATA 1 VALUE")
				.InnerText("INNER TEXT DATA 1")) // inner text
			.Node("data2", a => a
				.Attribute("name", "DATA 2 NAME")
				.Attribute("value", "DATA 2 VALUE")
				.InnerText("INNER TEXT DATA 2"))
			.Node("data3", a => a
				.Attribute("name", "DATA 3 NAME")
				.Attribute("value", "DATA 3 VALUE")
				.InnerText("INNER TEXT DATA 3")))
		.Node("element").In(n => n
			.Node("data1")
			.Node("data2")
			.Node("date3"))
		.Node("other")); // node without children

XmlDocument xml = fXml.GetXmlDocument(); // returns XmlDocument
```
