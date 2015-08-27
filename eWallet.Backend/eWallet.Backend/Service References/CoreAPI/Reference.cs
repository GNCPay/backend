﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eWallet.Backend.CoreAPI {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CoreAPI.IChannelAPI")]
    public interface IChannelAPI {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChannelAPI/Process", ReplyAction="http://tempuri.org/IChannelAPI/ProcessResponse")]
        string Process(string Request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChannelAPI/Process", ReplyAction="http://tempuri.org/IChannelAPI/ProcessResponse")]
        System.Threading.Tasks.Task<string> ProcessAsync(string Request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChannelAPI/Echo", ReplyAction="http://tempuri.org/IChannelAPI/EchoResponse")]
        string Echo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChannelAPI/Echo", ReplyAction="http://tempuri.org/IChannelAPI/EchoResponse")]
        System.Threading.Tasks.Task<string> EchoAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IChannelAPIChannel : eWallet.Backend.CoreAPI.IChannelAPI, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ChannelAPIClient : System.ServiceModel.ClientBase<eWallet.Backend.CoreAPI.IChannelAPI>, eWallet.Backend.CoreAPI.IChannelAPI {
        
        public ChannelAPIClient() {
        }
        
        public ChannelAPIClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ChannelAPIClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChannelAPIClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChannelAPIClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Process(string Request) {
            return base.Channel.Process(Request);
        }
        
        public System.Threading.Tasks.Task<string> ProcessAsync(string Request) {
            return base.Channel.ProcessAsync(Request);
        }
        
        public string Echo() {
            return base.Channel.Echo();
        }
        
        public System.Threading.Tasks.Task<string> EchoAsync() {
            return base.Channel.EchoAsync();
        }
    }
}
