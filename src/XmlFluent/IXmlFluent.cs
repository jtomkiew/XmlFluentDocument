using System;
using System.Xml;

namespace XmlFluent
{
    public interface IXmlFluent
    {
        string ToString();
        string GetOuterXml();
        XmlDocument GetXmlDocument();
    }

    public interface IXmlFluentRoot : IXmlFluent
    {
        IXmlFluentRoot XmlDeclaration();
        IXmlFluentRoot XmlDeclaration(string version, string encoding, string standalone);
        IXmlFluentNodeHierarchical RootNode(string name);

        IXmlFluentNodeHierarchical RootNode(string name, Func<IXmlFluentNodeParams, IXmlFluentNodeParams> func);
    }

    public interface IXmlFluentNodeHierarchical : IXmlFluent
    {
        IXmlFluentNodeFlat In(Func<IXmlFluentNodeFlat, IXmlFluentNodeFlat> func);
    }

    public interface IXmlFluentNodeFlat : IXmlFluent
    {
        IXmlFluentNode Node(string name);
        IXmlFluentNode Node(string name, Func<IXmlFluentNodeParams, IXmlFluentNodeParams> func);
    }

    public interface IXmlFluentNode : IXmlFluentNodeFlat, IXmlFluentNodeHierarchical
    {
    }

    public interface IXmlFluentNodeParams : IXmlFluent
    {
        IXmlFluentNodeParams InnerText(object text);
        IXmlFluentNodeParams InnerText(object text, bool useCData);
        IXmlFluentNodeParams Attribute(string name, object value);
    }

    public interface IXmlFluentDocument : IXmlFluentRoot, IXmlFluentNode, IXmlFluentNodeParams
    {
    }
}