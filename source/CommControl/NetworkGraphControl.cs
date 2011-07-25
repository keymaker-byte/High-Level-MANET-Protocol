/***************************************************************************
----------------------------------------------------------------------------
  This file is part of the HLMP API - Plug-ins Source Code.
  http://hlmprotocol.bicubic.cl
 
  Copyright (C) Bicubic TMG.  All rights reserved.
 
  This source code is intended only as a supplement to HLMP API 
  and/or on-line documentation.  
 
  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      
  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        
  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       
  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  
  AND PERMISSION FROM BICUBIC TMG.   

  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
----------------------------------------------------------------------------
****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommLayer;
using CommControl.Util;
using Microsoft.Glee.GraphViewerGdi;
using Microsoft.Glee.Drawing;

namespace CommControl
{
    /// <summary>
    /// Control grafico para la lista de usuarios de la red, bajo un modelo de grafo de la red
    /// </summary>
    public partial class NetworkGraphControl : UserControl
    {
        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        /// <param name="netUser">Un usuario de la red</param>
        private delegate void NetUserCallback(NetUser netUser);

        /// <summary>
        /// CallBack para llamadas a funciones de controles graficos en .NET
        /// </summary>
        private delegate void VoidArgumentCallBak();

        /// <summary>
        /// La lista de usuarios de la red
        /// </summary>
        private List<NetUserNode> netUsersNodes = new List<NetUserNode>();
        
        /// <summary>
        /// El control grafico de grafo
        /// </summary>
        private GViewer viewer = new GViewer();
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public NetworkGraphControl()
        {
            InitializeComponent();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox.Controls.Add(viewer);
            groupBox.ResumeLayout();
        }

        /// <summary>
        /// Agrega a un usuario al grafo de la red
        /// </summary>
        /// <param name="netUser">El usuario de la red</param>
        public void addUserHandler(NetUser netUser)
        {
            if (viewer.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(addUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                NetUserNode netUserNode = new NetUserNode();
                netUserNode.Id = netUser.Id;
                netUserNode.Name = netUser.Name;
                netUserNode.State = netUser.State;
                netUserNode.Quality = netUser.SignalQuality;
                for (int j = 0; j < netUser.NeighborhoodIds.Length; j++)
                {
                    int index = netUsersNodes.IndexOf(new NetUserNode(netUser.NeighborhoodIds[j]));
                    if (index != -1)
                    {
                        netUserNode.Links.Add(netUsersNodes[index]);
                    }
                }
                netUsersNodes.Add(netUserNode);
            }
        }

        /// <summary>
        /// Elimina a un usuario del grafo de la red
        /// </summary>
        /// <param name="netUser">El usuario de la red</param>
        public void removeUserHandler(NetUser netUser)
        {
            if (viewer.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(removeUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                int index = netUsersNodes.IndexOf(new NetUserNode(netUser.Id));
                if (index != -1)
                {
                    netUsersNodes.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario de la red en el grafo
        /// </summary>
        /// <param name="netUser">El usuario de la red</param>
        public void refreshUserHandler(NetUser netUser)
        {
            if (viewer.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(refreshUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                int index = netUsersNodes.IndexOf(new NetUserNode(netUser.Id));
                if (index != -1)
                {
                    NetUserNode netUserNode = netUsersNodes[index];
                    netUserNode.Id = netUser.Id;
                    netUserNode.Name = netUser.Name;
                    netUserNode.State = netUser.State;
                    netUserNode.Quality = netUser.SignalQuality;
                    netUserNode.Links.Clear();
                    for (int j = 0; j < netUser.NeighborhoodIds.Length; j++)
                    {
                        int i = netUsersNodes.IndexOf(new NetUserNode(netUser.NeighborhoodIds[j]));
                        if (i != -1)
                        {
                            netUserNode.Links.Add(netUsersNodes[i]);
                        }
                    }
                }
            } 
        }


        /// <summary>
        /// Actualiza los datos del usuario local en el grafo de la red
        /// </summary>
        /// <param name="netUser">El usario local</param>
        public void refreshLocalUserHandler(NetUser netUser)
        {
            if (viewer.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(refreshLocalUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                TreeNode newNode = new TreeNode(netUser.ToString());
                newNode.Tag = netUser;
                newNode.ImageIndex = 0;
                newNode.SelectedImageIndex = 0;

                int index = netUsersNodes.IndexOf(new NetUserNode(netUser.Id));
                if (index == -1)
                {
                    addUserHandler(netUser);
                }
                else
                {
                    refreshUserHandler(netUser);
                }
            }
        }

        /// <summary>
        /// Limpia el grafo de la red (elimina a todos los usuarios del grafo)
        /// </summary>
        public void clear()
        {
            if (viewer.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(clear);
                this.Invoke(d);
            }
            else
            {
                netUsersNodes.Clear();
                render();
            } 
        }

        /// <summary>
        /// Pinta el grafo de la red en el canvas correspondiente
        /// </summary>
        public void render() 
        {
            Graph graph = new Graph("MANET");
            foreach (NetUserNode netUserNode in netUsersNodes)
            {
                graph.AddNode(netUserNode.Id.ToString());
                Node node = graph.FindNode(netUserNode.Id.ToString());
                switch (netUserNode.State)
                {
                    case CommunicationQuality.NORMAL:
                        {
                            switch (netUserNode.Quality)
                            {
                                case NetUserQuality.NORMAL:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightGreen;
                                        break;
                                    }
                                case NetUserQuality.LOW:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightGreen;
                                        break;
                                    }
                                case NetUserQuality.CRITICAL:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightGreen;
                                        break;
                                    }
                            }
                            break;
                        }
                    case CommunicationQuality.OVERLOADED:
                        {
                            switch (netUserNode.Quality)
                            {
                                case NetUserQuality.NORMAL:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                                        break;
                                    }
                                case NetUserQuality.LOW:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                                        break;
                                    }
                                case NetUserQuality.CRITICAL:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                                        break;
                                    }
                            }
                            break;
                        }
                    case CommunicationQuality.CRITICAL:
                        {
                            switch (netUserNode.Quality)
                            {
                                case NetUserQuality.NORMAL:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                                        break;
                                    }
                                case NetUserQuality.LOW:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                                        break;
                                    }
                                case NetUserQuality.CRITICAL:
                                    {
                                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                                        break;
                                    }
                            }
                            break;
                        }
                }
                node.Attr.Fontcolor = Microsoft.Glee.Drawing.Color.DarkBlue;
                node.Attr.Fontsize = 6;
                node.Attr.Label = netUserNode.Name;
                node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Ellipse; 
                foreach (NetUserNode remoteNetUserNode in netUserNode.Links)
                {
                    graph.AddEdge(netUserNode.Id.ToString(), remoteNetUserNode.Id.ToString());
                }
            }
            viewer.Graph = graph;
            viewer.Refresh();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            render();
        }
    }
}
