﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.18063.
// 
#pragma warning disable 1591

namespace Neosinerji.BABOnlineTP.Business.eureko.basim {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    // CODEGEN: The optional WSDL extension element 'UsingAddressing' from namespace 'http://www.w3.org/2006/05/addressing/wsdl' was not handled.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="GetPdfServiceSoapBinding", Namespace="http://services.gt.com")]
    public partial class GetPdfServiceService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetPdfForInfoFormOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetPdfOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public GetPdfServiceService() {
            this.Url = global::Neosinerji.BABOnlineTP.Business.Properties.Settings.Default.Neosinerji_BABOnlineTP_Business_eureko_basim_GetPdfServiceService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetPdfForInfoFormCompletedEventHandler GetPdfForInfoFormCompleted;
        
        /// <remarks/>
        public event GetPdfCompletedEventHandler GetPdfCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("GetPdfForInfoForm", RequestNamespace="http://services.gt.com", ResponseNamespace="http://services.gt.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetPdfForInfoFormReturn", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] GetPdfForInfoForm([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)] PdfInputBean input) {
            object[] results = this.Invoke("GetPdfForInfoForm", new object[] {
                        input});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetPdfForInfoFormAsync(PdfInputBean input) {
            this.GetPdfForInfoFormAsync(input, null);
        }
        
        /// <remarks/>
        public void GetPdfForInfoFormAsync(PdfInputBean input, object userState) {
            if ((this.GetPdfForInfoFormOperationCompleted == null)) {
                this.GetPdfForInfoFormOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPdfForInfoFormOperationCompleted);
            }
            this.InvokeAsync("GetPdfForInfoForm", new object[] {
                        input}, this.GetPdfForInfoFormOperationCompleted, userState);
        }
        
        private void OnGetPdfForInfoFormOperationCompleted(object arg) {
            if ((this.GetPdfForInfoFormCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPdfForInfoFormCompleted(this, new GetPdfForInfoFormCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("GetPdf", RequestNamespace="http://services.gt.com", ResponseNamespace="http://services.gt.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("GetPdfReturn", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] GetPdf([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)] PdfInputBean input) {
            object[] results = this.Invoke("GetPdf", new object[] {
                        input});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetPdfAsync(PdfInputBean input) {
            this.GetPdfAsync(input, null);
        }
        
        /// <remarks/>
        public void GetPdfAsync(PdfInputBean input, object userState) {
            if ((this.GetPdfOperationCompleted == null)) {
                this.GetPdfOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPdfOperationCompleted);
            }
            this.InvokeAsync("GetPdf", new object[] {
                        input}, this.GetPdfOperationCompleted, userState);
        }
        
        private void OnGetPdfOperationCompleted(object arg) {
            if ((this.GetPdfCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPdfCompleted(this, new GetPdfCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://beans.gt.com")]
    public partial class PdfInputBean {
        
        private string polNumField;
        
        private string polGroupNumField;
        
        private string renewalNumField;
        
        private string endrsNumField;
        
        private string intEndrsNumField;
        
        private string chgSeqNumField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string polNum {
            get {
                return this.polNumField;
            }
            set {
                this.polNumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string polGroupNum {
            get {
                return this.polGroupNumField;
            }
            set {
                this.polGroupNumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string renewalNum {
            get {
                return this.renewalNumField;
            }
            set {
                this.renewalNumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string endrsNum {
            get {
                return this.endrsNumField;
            }
            set {
                this.endrsNumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string intEndrsNum {
            get {
                return this.intEndrsNumField;
            }
            set {
                this.intEndrsNumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string chgSeqNum {
            get {
                return this.chgSeqNumField;
            }
            set {
                this.chgSeqNumField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetPdfForInfoFormCompletedEventHandler(object sender, GetPdfForInfoFormCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPdfForInfoFormCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPdfForInfoFormCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetPdfCompletedEventHandler(object sender, GetPdfCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPdfCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPdfCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591