﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Neosinerji.BABOnlineTP.Business.AkSigorta {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/", ConfigurationName="AkSigorta.PolicePDFSorgulamaWebService")]
    public interface PolicePDFSorgulamaWebService {
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(baseWSType))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaResponse policePDFSorgulama(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaResponse> policePDFSorgulamaAsync(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/")]
    public partial class policePDFSorguInput : object, System.ComponentModel.INotifyPropertyChanged {
        
        private kanalBilgileriType kanalBilgileriField;
        
        private string policeNoField;
        
        private string basimDiliField;
        
        private string zeylSiraNoField;
        
        private basimTipi basimTipiField;
        
        private bool basimTipiFieldSpecified;
        
        private string satisIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public kanalBilgileriType kanalBilgileri {
            get {
                return this.kanalBilgileriField;
            }
            set {
                this.kanalBilgileriField = value;
                this.RaisePropertyChanged("kanalBilgileri");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string policeNo {
            get {
                return this.policeNoField;
            }
            set {
                this.policeNoField = value;
                this.RaisePropertyChanged("policeNo");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string basimDili {
            get {
                return this.basimDiliField;
            }
            set {
                this.basimDiliField = value;
                this.RaisePropertyChanged("basimDili");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string zeylSiraNo {
            get {
                return this.zeylSiraNoField;
            }
            set {
                this.zeylSiraNoField = value;
                this.RaisePropertyChanged("zeylSiraNo");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public basimTipi basimTipi {
            get {
                return this.basimTipiField;
            }
            set {
                this.basimTipiField = value;
                this.RaisePropertyChanged("basimTipi");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool basimTipiSpecified {
            get {
                return this.basimTipiFieldSpecified;
            }
            set {
                this.basimTipiFieldSpecified = value;
                this.RaisePropertyChanged("basimTipiSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string satisId {
            get {
                return this.satisIdField;
            }
            set {
                this.satisIdField = value;
                this.RaisePropertyChanged("satisId");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/")]
    public partial class kanalBilgileriType : baseWSType {
        
        private string kanalIdField;
        
        private string branchIdField;
        
        private string tokenField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string kanalId {
            get {
                return this.kanalIdField;
            }
            set {
                this.kanalIdField = value;
                this.RaisePropertyChanged("kanalId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string branchId {
            get {
                return this.branchIdField;
            }
            set {
                this.branchIdField = value;
                this.RaisePropertyChanged("branchId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string token {
            get {
                return this.tokenField;
            }
            set {
                this.tokenField = value;
                this.RaisePropertyChanged("token");
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(kanalBilgileriType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/")]
    public abstract partial class baseWSType : object, System.ComponentModel.INotifyPropertyChanged {
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/")]
    public enum basimTipi {
        
        /// <remarks/>
        POLICE_BASIM,
        
        /// <remarks/>
        BILGILENDIRME_FORMU,
        
        /// <remarks/>
        TAHSILAT_MAKBUZU,
        
        /// <remarks/>
        TUM_DOKUMANLAR,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="policePDFSorgulama", WrapperNamespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/", IsWrapped=true)]
    public partial class policePDFSorgulamaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorguInput policePDFSorgulama;
        
        public policePDFSorgulamaRequest() {
        }
        
        public policePDFSorgulamaRequest(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorguInput policePDFSorgulama) {
            this.policePDFSorgulama = policePDFSorgulama;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="policePDFSorgulamaResponse", WrapperNamespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/", IsWrapped=true)]
    public partial class policePDFSorgulamaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://policePDFSorgulama.police.webServices.aksigorta.tr.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] @return;
        
        public policePDFSorgulamaResponse() {
        }
        
        public policePDFSorgulamaResponse(byte[] @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PolicePDFSorgulamaWebServiceChannel : Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PolicePDFSorgulamaWebServiceClient : System.ServiceModel.ClientBase<Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService>, Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService {
        
        public PolicePDFSorgulamaWebServiceClient() {
        }
        
        public PolicePDFSorgulamaWebServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PolicePDFSorgulamaWebServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PolicePDFSorgulamaWebServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PolicePDFSorgulamaWebServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaResponse Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService.policePDFSorgulama(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest request) {
            return base.Channel.policePDFSorgulama(request);
        }
        
        public byte[] policePDFSorgulama(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorguInput policePDFSorgulama1) {
            Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest inValue = new Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest();
            inValue.policePDFSorgulama = policePDFSorgulama1;
            Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaResponse retVal = ((Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService)(this)).policePDFSorgulama(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaResponse> Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService.policePDFSorgulamaAsync(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest request) {
            return base.Channel.policePDFSorgulamaAsync(request);
        }
        
        public System.Threading.Tasks.Task<Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaResponse> policePDFSorgulamaAsync(Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorguInput policePDFSorgulama) {
            Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest inValue = new Neosinerji.BABOnlineTP.Business.AkSigorta.policePDFSorgulamaRequest();
            inValue.policePDFSorgulama = policePDFSorgulama;
            return ((Neosinerji.BABOnlineTP.Business.AkSigorta.PolicePDFSorgulamaWebService)(this)).policePDFSorgulamaAsync(inValue);
        }
    }
}
