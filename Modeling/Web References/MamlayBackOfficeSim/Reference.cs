﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3615
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.3615.
// 
#pragma warning disable 1591

namespace Modeling.MamlayBackOfficeSim {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SimulationBinding", Namespace="http://backoffice.mamlay.ru/simulation")]
    public partial class SimulationService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback niceStartOperationCompleted;
        
        private System.Threading.SendOrPostCallback getShoppingListOperationCompleted;
        
        private System.Threading.SendOrPostCallback receivingMaterialsOperationCompleted;
        
        private System.Threading.SendOrPostCallback getPlanOperationCompleted;
        
        private System.Threading.SendOrPostCallback getOrderStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback test1OperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SimulationService() {
            this.Url = global::Modeling.Properties.Settings.Default.Modeling_MamlayBackOfficeSim_SimulationService;
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
        public event niceStartCompletedEventHandler niceStartCompleted;
        
        /// <remarks/>
        public event getShoppingListCompletedEventHandler getShoppingListCompleted;
        
        /// <remarks/>
        public event receivingMaterialsCompletedEventHandler receivingMaterialsCompleted;
        
        /// <remarks/>
        public event getPlanCompletedEventHandler getPlanCompleted;
        
        /// <remarks/>
        public event getOrderStatusCompletedEventHandler getOrderStatusCompleted;
        
        /// <remarks/>
        public event test1CompletedEventHandler test1Completed;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://backoffice.mamlay.ru/simulation#niceStart", RequestNamespace="http://backoffice.mamlay.ru/simulation", ResponseNamespace="http://backoffice.mamlay.ru/simulation")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public bool niceStart(string date) {
            object[] results = this.Invoke("niceStart", new object[] {
                        date});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void niceStartAsync(string date) {
            this.niceStartAsync(date, null);
        }
        
        /// <remarks/>
        public void niceStartAsync(string date, object userState) {
            if ((this.niceStartOperationCompleted == null)) {
                this.niceStartOperationCompleted = new System.Threading.SendOrPostCallback(this.OnniceStartOperationCompleted);
            }
            this.InvokeAsync("niceStart", new object[] {
                        date}, this.niceStartOperationCompleted, userState);
        }
        
        private void OnniceStartOperationCompleted(object arg) {
            if ((this.niceStartCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.niceStartCompleted(this, new niceStartCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://backoffice.mamlay.ru/simulation#getShoppingList", RequestNamespace="http://backoffice.mamlay.ru/simulation", ResponseNamespace="http://backoffice.mamlay.ru/simulation")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public object getShoppingList(string date) {
            object[] results = this.Invoke("getShoppingList", new object[] {
                        date});
            return ((object)(results[0]));
        }
        
        /// <remarks/>
        public void getShoppingListAsync(string date) {
            this.getShoppingListAsync(date, null);
        }
        
        /// <remarks/>
        public void getShoppingListAsync(string date, object userState) {
            if ((this.getShoppingListOperationCompleted == null)) {
                this.getShoppingListOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetShoppingListOperationCompleted);
            }
            this.InvokeAsync("getShoppingList", new object[] {
                        date}, this.getShoppingListOperationCompleted, userState);
        }
        
        private void OngetShoppingListOperationCompleted(object arg) {
            if ((this.getShoppingListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getShoppingListCompleted(this, new getShoppingListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://backoffice.mamlay.ru/simulation#receivingMaterials", RequestNamespace="http://backoffice.mamlay.ru/simulation", ResponseNamespace="http://backoffice.mamlay.ru/simulation")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public bool receivingMaterials(string date, int deliveryId, object materials) {
            object[] results = this.Invoke("receivingMaterials", new object[] {
                        date,
                        deliveryId,
                        materials});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void receivingMaterialsAsync(string date, int deliveryId, object materials) {
            this.receivingMaterialsAsync(date, deliveryId, materials, null);
        }
        
        /// <remarks/>
        public void receivingMaterialsAsync(string date, int deliveryId, object materials, object userState) {
            if ((this.receivingMaterialsOperationCompleted == null)) {
                this.receivingMaterialsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnreceivingMaterialsOperationCompleted);
            }
            this.InvokeAsync("receivingMaterials", new object[] {
                        date,
                        deliveryId,
                        materials}, this.receivingMaterialsOperationCompleted, userState);
        }
        
        private void OnreceivingMaterialsOperationCompleted(object arg) {
            if ((this.receivingMaterialsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.receivingMaterialsCompleted(this, new receivingMaterialsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://backoffice.mamlay.ru/simulation#getPlan", RequestNamespace="http://backoffice.mamlay.ru/simulation", ResponseNamespace="http://backoffice.mamlay.ru/simulation")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public object getPlan(string date) {
            object[] results = this.Invoke("getPlan", new object[] {
                        date});
            return ((object)(results[0]));
        }
        
        /// <remarks/>
        public void getPlanAsync(string date) {
            this.getPlanAsync(date, null);
        }
        
        /// <remarks/>
        public void getPlanAsync(string date, object userState) {
            if ((this.getPlanOperationCompleted == null)) {
                this.getPlanOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetPlanOperationCompleted);
            }
            this.InvokeAsync("getPlan", new object[] {
                        date}, this.getPlanOperationCompleted, userState);
        }
        
        private void OngetPlanOperationCompleted(object arg) {
            if ((this.getPlanCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getPlanCompleted(this, new getPlanCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://backoffice.mamlay.ru/simulation#getOrderStatus", RequestNamespace="http://backoffice.mamlay.ru/simulation", ResponseNamespace="http://backoffice.mamlay.ru/simulation")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public object getOrderStatus(string date, int orderId) {
            object[] results = this.Invoke("getOrderStatus", new object[] {
                        date,
                        orderId});
            return ((object)(results[0]));
        }
        
        /// <remarks/>
        public void getOrderStatusAsync(string date, int orderId) {
            this.getOrderStatusAsync(date, orderId, null);
        }
        
        /// <remarks/>
        public void getOrderStatusAsync(string date, int orderId, object userState) {
            if ((this.getOrderStatusOperationCompleted == null)) {
                this.getOrderStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetOrderStatusOperationCompleted);
            }
            this.InvokeAsync("getOrderStatus", new object[] {
                        date,
                        orderId}, this.getOrderStatusOperationCompleted, userState);
        }
        
        private void OngetOrderStatusOperationCompleted(object arg) {
            if ((this.getOrderStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getOrderStatusCompleted(this, new getOrderStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://backoffice.mamlay.ru/simulation#test1", RequestNamespace="http://backoffice.mamlay.ru/simulation", ResponseNamespace="http://backoffice.mamlay.ru/simulation")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public int test1(string id) {
            object[] results = this.Invoke("test1", new object[] {
                        id});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void test1Async(string id) {
            this.test1Async(id, null);
        }
        
        /// <remarks/>
        public void test1Async(string id, object userState) {
            if ((this.test1OperationCompleted == null)) {
                this.test1OperationCompleted = new System.Threading.SendOrPostCallback(this.Ontest1OperationCompleted);
            }
            this.InvokeAsync("test1", new object[] {
                        id}, this.test1OperationCompleted, userState);
        }
        
        private void Ontest1OperationCompleted(object arg) {
            if ((this.test1Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.test1Completed(this, new test1CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void niceStartCompletedEventHandler(object sender, niceStartCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class niceStartCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal niceStartCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void getShoppingListCompletedEventHandler(object sender, getShoppingListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getShoppingListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getShoppingListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public object Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((object)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void receivingMaterialsCompletedEventHandler(object sender, receivingMaterialsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class receivingMaterialsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal receivingMaterialsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void getPlanCompletedEventHandler(object sender, getPlanCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getPlanCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getPlanCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public object Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((object)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void getOrderStatusCompletedEventHandler(object sender, getOrderStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getOrderStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getOrderStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public object Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((object)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void test1CompletedEventHandler(object sender, test1CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class test1CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal test1CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591