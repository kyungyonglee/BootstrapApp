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
using Brunet.Applications;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
 
namespace Brunet.Applications.Examples {
  /// <summary>This class show an example HelloWorld of Brunet using
  /// IRpcHandler.  We inherit BasicNode and IRpcHandler.  BasicNode
  /// provides access to Brunet in a clean manner and IRpcHandler allows
  /// this class to be an end point for XML Rpc call.</summary>
  public class HelloWorldRpcHandler : BasicNode, IRpcHandler{
    /// <summary>The only parameter to the constructor is a valid NodeConfig.
    public HelloWorldRpcHandler(NodeConfig node_config) : base(node_config) {
    }
    
    /**
       * <summary>This is the only method declared by IRpcHandler.  All xml rpc calls
       * which start with ("HwRpc.") will arrive here. It simply prints the sender address and method name to the console.
       * It also sends result to the sender. The result is input value from the sender</summary>
       * @param caller the ISender that sends to the Node that made the RPC call
       * @param method the part after the first "." in the method call
       * @param arguments a list of arguments passed
       * @param request_state used to send the response via RpcManager.SendResult
       */
    public void HandleRpc(ISender caller, string method, IList arguments, object request_state){
      Console.WriteLine(caller + ": " + method + " : " + Encoding.ASCII.GetString(arguments[0] as byte[]));
      _node.Rpc.SendResult(request_state, arguments[0]);
    }
 
    /// <summary>This is the work horse method.</summary>
    public override void Run() {
      // This handles the whole process of preparing the Brunet.Node.
      CreateNode();

      // Services include XmlRpcManager and Dht over XmlRpcManager
      StartServices();

      //It registers this class to the currently connected brunet node's RpcManager Class. 
      //By registering this class, all rpc call whose prefix is "HwRpc" will be forwarded to this class.
      _node.Rpc.AddHandler("HwRpc", this);

      // Start the Brunet.Node and allow it to connect to remote nodes
      Thread thread = new Thread(_node.Connect);
      thread.Start();

      Console.WriteLine("Your address is: " + _node.Address + "\n");
 
      // We will continue on, until we get to the Disconnected states. Assumming
      // you are running this on a supported platform, that would be triggered
      // initially by ctrl-c
      while(_node.ConState != Node.ConnectionState.Disconnected) {
        Console.ReadLine();      
      }
 
      // Stops the XmlRpcManager and associated services
      StopServices();
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
 

