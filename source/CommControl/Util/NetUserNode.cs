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
using System.Text;
using System.Collections;

namespace CommControl.Util
{
    /// <summary>
    /// Representa a un usuario de red para uso del grafo de la MANET
    /// </summary>
    internal class NetUserNode
    {
        /// <summary>
        /// El nombre del usuario
        /// </summary>
        private String _name;

        /// <summary>
        /// El id del usuario
        /// </summary>
        private Guid _id;

        /// <summary>
        /// La lista de usuarios devinos
        /// </summary>
        private List<NetUserNode> _links;

        /// <summary>
        /// Es estado de trafico del usuario
        /// Un valor de CommunicationQuality
        /// </summary>
        private Int32 _state;

        /// <summary>
        /// El estado de la calidad de la señal del usuario
        /// Un valor de NetUserQuality
        /// </summary>
        private Int32 _quality;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NetUserNode()
        {
            this.Links = new List<NetUserNode>();
        }

        /// <summary>
        /// Constructor parametrizado
        /// </summary>
        /// <param name="id">El id del usuario de red</param>
        public NetUserNode(Guid id)
        {
            this.Id = id;
            this.Links = new List<NetUserNode>();
        }

        /// <summary>
        /// El nombre del usuario
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// El id del usuario
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
                
        /// <summary>
        /// La lista de usuarios vecinos
        /// </summary>
        public List<NetUserNode> Links
        {
            get { return _links; }
            set { _links = value; }
        }

        /// <summary>
        /// Es estado de trafico del usuario
        /// Un valor de CommunicationQuality
        /// </summary>
        public Int32 State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// El estado de la calidad de la señal del usuario
        /// Un valor de NetUserQuality
        /// </summary>
        public Int32 Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }

        /// <summary>
        /// Sobreescribe el método equals
        /// </summary>
        /// <param name="obj">El objeto que se intenta comparar</param>
        /// <returns>true si ambos usuarios tienen el mismo id, false si no</returns>
        public override bool Equals(object obj)
        {
            return this.Id.Equals(((NetUserNode)obj).Id);
        }

        /// <summary>
        /// Necesario para el metodo equals
        /// </summary>
        /// <returns>El hashcode base</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
