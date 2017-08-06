using System;
using System.Collections.Generic;
using System.Xml;

namespace XmlFluentDocument
{
    /// <summary>
    ///     Based on XmlOutput-Nuget: https://github.com/cigano/XmlOutput-Nuget (origin: https://github.com/improvedk/XmlOutput)
    /// </summary>
    public class XmlFluentDocument : IXmlFluentDocument
    {
        // The internal XmlDocument that holds the complete structure.
        private readonly XmlDocument _xd = new XmlDocument();

        // A stack representing the hierarchy of nodes added. nodeStack.Peek() will always be the current node scope.
        private readonly Stack<XmlNode> _nodeStack = new Stack<XmlNode>();

        // The current node. If null, the current node is the XmlDocument itself.
        private XmlNode _currentNode;

        // Whether the next node should be created in the scope of the current node.
        private bool _nextNodeWithin;

        // Whether the Xml declaration has been added to the document
        private bool _xmlDeclarationHasBeenAdded;

        protected XmlFluentDocument()
        {
        }

        #region StaticConstructor

        /// <summary>
        ///     Creates new XmlFluentDocument instance.
        /// </summary>
        /// <returns></returns>
        public static IXmlFluentRoot New()
        {
            return new XmlFluentDocument();
        }

        #endregion

        #region General

        /// <summary>
        ///     Overrides ToString to easily return the current outer Xml
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetOuterXml();
        }

        /// <summary>
        ///     Returns the string representation of the XmlDocument.
        /// </summary>
        /// <returns>A string representation of the XmlDocument.</returns>
        public string GetOuterXml()
        {
            return _xd.OuterXml;
        }

        /// <summary>
        ///     Returns the XmlDocument
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlDocument()
        {
            return _xd;
        }

        #endregion

        #region RootNodeAndDeclaration

        /// <summary>
        ///     Adds an XML declaration with the most common values.
        /// </summary>
        /// <returns>this</returns>
        public IXmlFluentRoot XmlDeclaration()
        {
            return XmlDeclaration("1.0", "utf-8", "");
        }

        /// <summary>
        ///     Adds an XML declaration to the document.
        /// </summary>
        /// <param name="version">The version of the XML document.</param>
        /// <param name="encoding">The encoding of the XML document.</param>
        /// <param name="standalone">Whether the document is standalone or not. Can be yes/no/(null || "").</param>
        /// <returns>this</returns>
        public IXmlFluentRoot XmlDeclaration(string version, string encoding, string standalone)
        {
            // We can't add an XmlDeclaration once nodes have been added, as the standard declaration will already have been added
            if (_nodeStack.Count > 0)
                throw new InvalidOperationException(
                    "Cannot add XmlDeclaration once nodes have been added to the XmlFluentDocument.");

            // Create & add the XmlDeclaration
            var xdec = _xd.CreateXmlDeclaration(version, encoding, standalone);
            _xd.AppendChild(xdec);

            _xmlDeclarationHasBeenAdded = true;

            return this;
        }

        /// <summary>
        ///     Creates root node.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IXmlFluentNodeHierarchical RootNode(string name)
        {
            Node(name);
            return this;
        }

        /// <summary>
        ///     Creates root node with attributes specified by function func.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public IXmlFluentNodeHierarchical RootNode(string name, Func<IXmlFluentNodeParams, IXmlFluentNodeParams> func)
        {
            RootNode(name);
            func.Invoke(this);
            return this;
        }

        #endregion

        #region NodeStructureMethods

        /// <summary>
        ///     Creates a node. It'll be appended as a child of the current node.
        /// </summary>
        /// <param name="name">The name of the node to create.</param>
        /// <returns>this</returns>
        public IXmlFluentNode Node(string name)
        {
            XmlNode xn = _xd.CreateElement(name);

            // If nodeStack.Count == 0, no nodes have been added, thus the scope is the XmlDocument itself.
            if (_nodeStack.Count == 0)
            {
                // If an XmlDeclaration has not been added, add the standard declaration
                if (!_xmlDeclarationHasBeenAdded)
                    XmlDeclaration();

                // Add the child element to the XmlDocument directly
                _xd.AppendChild(xn);

                // Automatically change scope to the root DocumentElement.
                _nodeStack.Push(xn);
            }
            else
            {
                // If this node should be created within the scope of the current node, change scope to the current node before adding the node to the scope element.
                if (_nextNodeWithin)
                {
                    _nodeStack.Push(_currentNode);

                    _nextNodeWithin = false;
                }

                _nodeStack.Peek().AppendChild(xn);
            }

            _currentNode = xn;

            return this;
        }

        /// <summary>
        ///     Creates a node with attributes specified by function func. It'll be appended as a child of the current node.
        /// </summary>
        /// <param name="name">The name of the node to create.</param>
        /// <param name="func"></param>
        /// <returns>this</returns>
        public IXmlFluentNode Node(string name, Func<IXmlFluentNodeParams, IXmlFluentNodeParams> func)
        {
            Node(name);
            func.Invoke(this);
            return this;
        }

        /// <summary>
        ///     Steps inside the current node.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public IXmlFluentNodeFlat In(Func<IXmlFluentNodeFlat, IXmlFluentNodeFlat> func)
        {
            _nextNodeWithin = true;

            func.Invoke(this);

            if (_nextNodeWithin)
                _nextNodeWithin = false;
            else
                _nodeStack.Pop();

            return this;
        }

        #endregion

        #region NodeMethods

        /// <summary>
        ///     Sets the InnerText of the current node using CData.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IXmlFluentNodeParams InnerText(object text)
        {
            return InnerText(text.ToString(), true);
        }

        /// <summary>
        ///     Sets the InnerText of the current node.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <param name="useCData"></param>
        /// <returns>this</returns>
        public IXmlFluentNodeParams InnerText(object text, bool useCData)
        {
            if (useCData)
                _currentNode.AppendChild(_xd.CreateCDataSection(text.ToString()));
            else
                _currentNode.AppendChild(_xd.CreateTextNode(text.ToString()));

            return this;
        }

        /// <summary>
        ///     Adds an attribute to the current node.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this</returns>
        public IXmlFluentNodeParams Attribute(string name, object value)
        {
            var xa = _xd.CreateAttribute(name);
            xa.Value = value.ToString();

            _currentNode.Attributes.Append(xa);

            return this;
        }

        #endregion
    }
}