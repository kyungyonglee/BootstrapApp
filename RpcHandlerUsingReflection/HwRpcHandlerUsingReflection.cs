/*
Copyright (C) 2009 David Wolinsky <davidiw@ufl.edu>, University of Florida
 
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
 
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
*/
 
using Brunet;
using Brunet.Messaging;
using Brunet.Applications;
using Brunet.Util;
using Brunet.Symphony;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
 
namespace Brunet.Applications.Examples {
  /// <summary>This class show an example HelloWorld of Brunet using
  /// RpcHandler Reflection.  We inherit only BasicNode.  IRpcHanlder is inherited in  
  /// RpcManager. BasicNode provides access to Brunet in a clean manner and RpcManager Reflection allows
  /// this class to be an end point for XML Rpc call without inheriting IRpcHandler clas.</summary>
  public class HelloWorldRpcHandler : BasicNode{
    /// <summary>The only parameter to the constructor is a valid NodeConfig.
    public HelloWorldRpcHandler(NodeConfig node_config) : base(node_config) {
    }

    ///<summary>This method is called when "HwRpc.Test" rpc call comes in. Thought this class
    /// does not inherit IRpcHanlder class, it uses RpcManager.HandleRpc method using Reflection</summary>
    ///<param name="arg" >rpc call argument from user input</param>
    public object Test(byte[] arg){
      Console.WriteLine("HelloWorld Rpc Handler is called through reflection");
      return arg;
      
    }
    /// <summary>This is the work horse method.</summary>
    public override void Run() {
      // This handles the whole process of preparing the Brunet.Node.
      _app_node = CreateNode(_node_config);

      //It registers this class to the currently connected brunet node's RpcManager Class. 
      //By registering this class, all rpc call whose prefix is "HwRpc" will be forwarded to this class.
      _app_node.Node.Rpc.AddHandler("HwRpc", this);

      // Start the Brunet.Node and allow it to connect to remote nodes
      Thread thread = new Thread(_app_node.Node.Connect);
      thread.Start();

      Console.WriteLine("Your address is: " + _app_node.Node.Address + "\n");
 
      // We will continue on, until we get to the Disconnected states. Assumming
      // you are running this on a supported platform, that would be triggered
      // initially by ctrl-c
      while(_app_node.Node.ConState != Node.ConnectionState.Disconnected) {
        Console.ReadLine();      
      }
      }
  }
  
  public class Runner {
    public static int Main(string [] args) {
      // We need a valid NodeConfig, these are the proper steps to ensure we get one
      if(args.Length < 1 || !File.Exists(args[0])) {
        Console.WriteLine("First argument must be a NodeConfig");
        return -1;
      }
 
      NodeConfig node_config = null;
      try {
        node_config = Utils.ReadConfig<NodeConfig>(args[0]);
      } catch (Exception e) {
        Console.WriteLine("Invalid NodeConfig file:");
        Console.WriteLine("\t" + e.Message);
        return -1;
      }
 
      // Instantiate a new inherited node of your choice
      HelloWorldRpcHandler hwn = new HelloWorldRpcHandler(node_config);
      // And run it... this hijacks the current thread, we'll return once the node disconnects
      hwn.Run();
      
      Console.WriteLine("Exiting...");
 
      return 0;
    }
  }
 }
 

