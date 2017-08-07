using System;
using System.Xml;

namespace XmlFluent
{
    public interface IXmlFluentDocument : IXmlFluentRoot, IXmlFluentNode, IXmlFluentNodeParams
    {
    }

    public interface IXmlFluent
    {
        /// <summary>
        ///     Overrides ToString to easily return the current outer Xml
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        ///     Returns the string representation of the XmlDocument.
        /// </summary>
        /// <returns>A string representation of the XmlDocument.</returns>
        string GetOuterXml();

        /// <summary>
        ///     Returns the XmlDocument
        /// </summary>
        /// <returns></returns>
        XmlDocument GetXmlDocument();
    }

    public interface IXmlFluentRoot : IXmlFluent
    {
        /// <summary>
        ///     Adds an XML declaration with the most common values.
        /// </summary>
        /// <returns>this</returns>
        IXmlFluentRoot XmlDeclaration();

        /// <summary>
        ///     Adds an XML declaration to the document.
        /// </summary>
        /// <param name="version">The version of the XML document.</param>
        /// <param name="encoding">The encoding of the XML document.</param>
        /// <param name="standalone">Whether the document is standalone or not. Can be yes/no/(null || "").</param>
        /// <returns>this</returns>
        IXmlFluentRoot XmlDeclaration(string version, string encoding, string standalone);

        /// <summary>
        ///     Creates root node.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IXmlFluentNodeHierarchical RootNode(string name);

        /// <summary>
        ///     Creates root node with attributes specified by function.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        IXmlFluentNodeHierarchical RootNode(string name, Func<IXmlFluentNodeParams, IXmlFluentNodeParams> func);
    }

    public interface IXmlFluentNodeHierarchical : IXmlFluent
    {
        /// <summary>
        ///     Steps inside the current node.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IXmlFluentNodeFlat In(Func<IXmlFluentNodeFlat, IXmlFluentNodeFlat> func);
    }

    public interface IXmlFluentNodeFlat : IXmlFluent
    {
        /// <summary>
        ///     Creates a node. It'll be appended as a child of the current node.
        /// </summary>
        /// <param name="name">The name of the node to create.</param>
        /// <returns>this</returns>
        IXmlFluentNode Node(string name);

        /// <summary>
        ///     Creates a node with attributes specified by function. It'll be appended as a child of the current node.
        /// </summary>
        /// <param name="name">The name of the node to create.</param>
        /// <param name="func"></param>
        /// <returns>this</returns>
        IXmlFluentNode Node(string name, Func<IXmlFluentNodeParams, IXmlFluentNodeParams> func);
    }

    public interface IXmlFluentNode : IXmlFluentNodeFlat, IXmlFluentNodeHierarchical
    {
    }

    public interface IXmlFluentNodeParams : IXmlFluent
    {
        /// <summary>
        ///     Sets the InnerText of the current node using CData.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        IXmlFluentNodeParams InnerText(object text);

        /// <summary>
        ///     Sets the InnerText of the current node.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <param name="useCData"></param>
        /// <returns>this</returns>
        IXmlFluentNodeParams InnerText(object text, bool useCData);

        /// <summary>
        ///     Adds an attribute to the current node.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this</returns>
        IXmlFluentNodeParams Attribute(string name, object value);
    }
}