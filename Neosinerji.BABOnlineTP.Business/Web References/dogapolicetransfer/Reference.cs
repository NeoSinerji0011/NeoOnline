﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace Neosinerji.BABOnlineTP.Business.dogapolicetransfer {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="AcenteBilgiServisleriSoap", Namespace="http://tempuri.org/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ServisCevap))]
    public partial class AcenteBilgiServisleri : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback PoliceListesiOperationCompleted;
        
        private System.Threading.SendOrPostCallback TekPoliceOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public AcenteBilgiServisleri() {
            this.Url = global::Neosinerji.BABOnlineTP.Business.Properties.Settings.Default.Neosinerji_BABOnlineTP_Business_dogapolicetransfer_AcenteBilgiServisleri;
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
        public event PoliceListesiCompletedEventHandler PoliceListesiCompleted;
        
        /// <remarks/>
        public event TekPoliceCompletedEventHandler TekPoliceCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/PoliceListesi", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public GeriyePoliceTransferCevap PoliceListesi(string kullaniciAdi, string parola, string acenteNo, string baslangicBrans, string bitisBrans, string baslangicTarihi, string bitisTarihi) {
            object[] results = this.Invoke("PoliceListesi", new object[] {
                        kullaniciAdi,
                        parola,
                        acenteNo,
                        baslangicBrans,
                        bitisBrans,
                        baslangicTarihi,
                        bitisTarihi});
            return ((GeriyePoliceTransferCevap)(results[0]));
        }
        
        /// <remarks/>
        public void PoliceListesiAsync(string kullaniciAdi, string parola, string acenteNo, string baslangicBrans, string bitisBrans, string baslangicTarihi, string bitisTarihi) {
            this.PoliceListesiAsync(kullaniciAdi, parola, acenteNo, baslangicBrans, bitisBrans, baslangicTarihi, bitisTarihi, null);
        }
        
        /// <remarks/>
        public void PoliceListesiAsync(string kullaniciAdi, string parola, string acenteNo, string baslangicBrans, string bitisBrans, string baslangicTarihi, string bitisTarihi, object userState) {
            if ((this.PoliceListesiOperationCompleted == null)) {
                this.PoliceListesiOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPoliceListesiOperationCompleted);
            }
            this.InvokeAsync("PoliceListesi", new object[] {
                        kullaniciAdi,
                        parola,
                        acenteNo,
                        baslangicBrans,
                        bitisBrans,
                        baslangicTarihi,
                        bitisTarihi}, this.PoliceListesiOperationCompleted, userState);
        }
        
        private void OnPoliceListesiOperationCompleted(object arg) {
            if ((this.PoliceListesiCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PoliceListesiCompleted(this, new PoliceListesiCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TekPolice", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public GeriyePoliceTransferCevap TekPolice(string kullaniciAdi, string parola, string acenteNo, string brans, string policeNo, string tecditNo, string zeyilNo) {
            object[] results = this.Invoke("TekPolice", new object[] {
                        kullaniciAdi,
                        parola,
                        acenteNo,
                        brans,
                        policeNo,
                        tecditNo,
                        zeyilNo});
            return ((GeriyePoliceTransferCevap)(results[0]));
        }
        
        /// <remarks/>
        public void TekPoliceAsync(string kullaniciAdi, string parola, string acenteNo, string brans, string policeNo, string tecditNo, string zeyilNo) {
            this.TekPoliceAsync(kullaniciAdi, parola, acenteNo, brans, policeNo, tecditNo, zeyilNo, null);
        }
        
        /// <remarks/>
        public void TekPoliceAsync(string kullaniciAdi, string parola, string acenteNo, string brans, string policeNo, string tecditNo, string zeyilNo, object userState) {
            if ((this.TekPoliceOperationCompleted == null)) {
                this.TekPoliceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTekPoliceOperationCompleted);
            }
            this.InvokeAsync("TekPolice", new object[] {
                        kullaniciAdi,
                        parola,
                        acenteNo,
                        brans,
                        policeNo,
                        tecditNo,
                        zeyilNo}, this.TekPoliceOperationCompleted, userState);
        }
        
        private void OnTekPoliceOperationCompleted(object arg) {
            if ((this.TekPoliceCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TekPoliceCompleted(this, new TekPoliceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GeriyePoliceTransferCevap : ServisCevap {
        
        private System.Xml.XmlNode policelerField;
        
        /// <remarks/>
        public System.Xml.XmlNode Policeler {
            get {
                return this.policelerField;
            }
            set {
                this.policelerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GeriyePoliceTransferCevap))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServisCevap {
        
        private string hataField;
        
        private bool basariliField;
        
        /// <remarks/>
        public string Hata {
            get {
                return this.hataField;
            }
            set {
                this.hataField = value;
            }
        }
        
        /// <remarks/>
        public bool Basarili {
            get {
                return this.basariliField;
            }
            set {
                this.basariliField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void PoliceListesiCompletedEventHandler(object sender, PoliceListesiCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PoliceListesiCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PoliceListesiCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GeriyePoliceTransferCevap Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GeriyePoliceTransferCevap)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void TekPoliceCompletedEventHandler(object sender, TekPoliceCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TekPoliceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal TekPoliceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GeriyePoliceTransferCevap Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GeriyePoliceTransferCevap)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591