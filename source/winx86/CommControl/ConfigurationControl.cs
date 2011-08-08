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
using System.Text;
using System.Windows.Forms;
using NetLayer;
using CommLayer;
using System.Net;
using System.IO;

namespace CommControl
{
    /// <summary>
    /// Control gráfico para el ingreso de datos de configuración (formulario)
    /// </summary>
    public partial class ConfigurationControl : UserControl
    {
        /// <summary>
        /// Los datos de configuración
        /// </summary>
        private Configuration _configurationData;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConfigurationControl()
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
            //inicializa el combo de adaptadores de red
            try
            {
                List<NetworkAdapter> adapters = SystemHandler.getNetworkAdapters();
                foreach (NetworkAdapter adapter in adapters)
                {
                    comboBoxAdapter.Items.Add(adapter);
                    //Intenta seleccionar un adaptador que posea el nombre Wireless, posiblemente la tarjeta de red inalambrica
                    if (ConfigurationData.NetData.NetworkAdapter != null)
                    {
                        comboBoxAdapter.SelectedItem = ConfigurationData.NetData.NetworkAdapter;
                    }
                    else if (adapter.Description.IndexOf("Wireless") != -1)
                    {
                        comboBoxAdapter.SelectedItem = adapter;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al listar adaptadores de red. " + e.Message);
                
            }

            //inicializa los formularios
            textBoxName.Text = ConfigurationData.NetUser.Name;
            if (ConfigurationData.NetData.OpSystem.Equals(OpSystemType.WINXPSP3))
            {
                radioButtonXPSP3.Checked = true;
            }
            else if (ConfigurationData.NetData.OpSystem.Equals(OpSystemType.WIN7))
            {
                radioButtonVista.Checked = true;
            }
            comboBoxAdapter.SelectedItem = ConfigurationData.NetData.NetworkAdapter;
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
            //Dispositivo
            if (comboBoxAdapter.SelectedIndex != -1)
            {
                ConfigurationData.NetData.NetworkAdapter = (NetworkAdapter)comboBoxAdapter.SelectedItem;
            }
            else
            {
                MessageBox.Show("Debe seleccionar un Dispositivo de red.");
                return false;
            }
            //Modo
            if (radioButtonVista.Checked)
            {
                ConfigurationData.NetData.OpSystem = OpSystemType.WIN7;
            }
            else if (radioButtonXPSP3.Checked)
            {
                ConfigurationData.NetData.OpSystem = OpSystemType.WINXPSP3;
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
