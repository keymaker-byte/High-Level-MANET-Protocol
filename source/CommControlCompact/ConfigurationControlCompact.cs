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
using NetLayerCompact;
using CommLayerCompact;
using System.Net;
using System.IO;

namespace CommControlCompact
{
    /// <summary>
    /// Control gráfico para el ingreso de datos de configuración (formulario)
    /// </summary>
    public partial class ConfigurationControlCompact : UserControl
    {
        /// <summary>
        /// Los datos de configuración
        /// </summary>
        private Configuration _configurationData;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConfigurationControlCompact()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Los datos de configuración editados en el formulario (null si se ha cancelado).
        /// </summary>
        public Configuration ConfigurationData
        {
            get { return _configurationData; }
            set { _configurationData = value;  }
        }

        /// <summary>
        /// Inicializa los datos del formulario con la información de la propiedad ConfigurationData
        /// </summary>
        public void init()
        {
            //Crea la nueva clase de configuracion, si no existe
            if (ConfigurationData == null)
            {
                ConfigurationData = new Configuration();
            }

            //inicializa los formularios
            textBoxName.Text = ConfigurationData.NetUser.Name;
        }

        /// <summary>
        /// Guarda la información actual del formulario en la propiedad ConfigurationData
        /// </summary>
        /// <returns>true si el formulario ha sido llenado correctamente y ha sido guardado, false si no
        /// Si no ha sido bien llenado, se emite una ventana modal con el mensaje correspondiente</returns>
        public bool aceptar()
        {
            //Nombre
            if (textBoxName.Text != "")
            {
                ConfigurationData.NetUser.Name = textBoxName.Text;
            }
            else
            {
                MessageBox.Show("Debe especificar un Nombre de Usuario válido.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cancela los cambios hechos a la configuración, la propiedad ConfigurationData se vuelve null
        /// </summary>
        public void cancelar()
        {
            ConfigurationData = null;
        }
    }
}
