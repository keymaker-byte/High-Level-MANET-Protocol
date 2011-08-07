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
using System.Threading;
using System.Windows.Forms;
using CommLayer;
using CommControl.Util;

namespace CommControl
{
    /// <summary>
    /// Control gráfico para la lista de usuarios de la red
    /// </summary>
    public partial class NetUserControlCompact : UserControl
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
        /// Default Constructor
        /// </summary>>
        public NetUserControlCompact()
        {
            InitializeComponent();
        }

        /// <summary>
        /// El tamaño de la imágen de los íconos de los usuarios
        /// </summary>
        public Size ImageSize
        {
            get { return iconList.ImageSize; }
            set { iconList.ImageSize = value; }
        }

        /// <summary>
        /// El tamaño de la indentación de los nodos en el arbol
        /// </summary>
        public Int32 Indent
        {
            get { return treeView.Indent; }
            set { treeView.Indent= value; }
        }

        /// <summary>
        /// Agrega a un usuario de la red al árbol de usuarios
        /// </summary>
        /// <param name="netUser">El nuevo usuario que ha ingresado a la red</param>
        public void addUserHandler(NetUser netUser)
        {
            if (treeView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(addUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                TreeNode newNode = new TreeNode(netUser.ToString());
                newNode.Tag = netUser;
                newNode.ImageIndex = netUser.SignalQuality;
                newNode.SelectedImageIndex = netUser.SignalQuality;

                int index = -1;
                foreach (TreeNode node in treeView.Nodes)
                {
                    NetUser tagNetUSer = (NetUser)node.Tag;
                    if (tagNetUSer.Id == netUser.Id)
                    {
                        index = node.Index;
                        break;
                    }
                }
                if (index == -1)
                {
                    treeView.Nodes.Add(newNode);
                }
                else
                {
                    refreshUserHandler(netUser);
                }
            }

        }

        /// <summary>
        /// Elimina a un usuario de la red del arbol de usuarios
        /// </summary>
        /// <param name="netUser">El usuario que salido de la red</param>
        public void removeUserHandler(NetUser netUser)
        {
            if (treeView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(removeUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                int index = -1;
                foreach (TreeNode node in treeView.Nodes)
                {
                    NetUser tagNetUser = (NetUser)node.Tag;
                    if (tagNetUser.Id == netUser.Id)
                    {
                        index = node.Index;
                        break;
                    }
                }
                if (index != -1)
                {
                    treeView.Nodes.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario en el Árbol de usuarios
        /// </summary>
        /// <param name="netUser">El usuario que ha actualizado sus datos o estado</param>
        public void refreshUserHandler(NetUser netUser)
        {
            if (treeView.InvokeRequired)
            {
                NetUserCallback d = new NetUserCallback(refreshUserHandler);
                this.Invoke(d, new object[] { netUser });
            }
            else
            {
                foreach (TreeNode node in treeView.Nodes)
                {
                    NetUser tagNetUSer = (NetUser)node.Tag;
                    if (tagNetUSer.Id == netUser.Id)
                    {
                        if (!node.Text.Equals(netUser.ToString()))
                        {
                            node.Text = netUser.ToString();
                        }
                        if (!node.Tag.Equals(netUser))
                        {
                            node.Tag = netUser;
                        }
                        if (!node.ImageIndex.Equals(tagNetUSer.SignalQuality))
                        {
                            node.ImageIndex = tagNetUSer.SignalQuality;
                            node.SelectedImageIndex = tagNetUSer.SignalQuality;
                        }                        
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Ingresa o actualiza los datos del usuario local en el Árbol de usuarios
        /// </summary>
        /// <param name="netUser">El usario local</param>
        public void refreshLocalUserHandler(NetUser netUser)
        {
            if (treeView.InvokeRequired)
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

                int index = -1;
                foreach (TreeNode node in treeView.Nodes)
                {
                    NetUser tagNetUSer = (NetUser)node.Tag;
                    if (tagNetUSer.Id == netUser.Id)
                    {
                        index = node.Index;
                        break;
                    }
                }
                if (index == -1)
                {
                    treeView.Nodes.Add(newNode);
                }
                else
                {
                    TreeNode node = treeView.Nodes[index];
                    NetUser tagNetUSer = (NetUser)node.Tag;
                    if (!node.Text.Equals(netUser.ToString()))
                    {
                        node.Text = netUser.ToString();
                    }
                    if (!node.Tag.Equals(netUser))
                    {
                        node.Tag = netUser;
                    }
                }
            }
        }

        /// <summary>
        /// Remueve a todos los usuarios de la red del árbol de usuarios
        /// </summary>
        public void clear()
        {
            if (treeView.InvokeRequired)
            {
                VoidArgumentCallBak d = new VoidArgumentCallBak(clear);
                this.Invoke(d);
            }
            else
            {
                treeView.Nodes.Clear();
            }
        }
    }
}
