﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CTR;
using DTO;

namespace BlessFarma
{
    public partial class AgregarPedido : System.Web.UI.Page
    {
        DTO_ProductoListaCompra objProductoLC;
        protected void Page_Load(object sender, EventArgs e)
        {
            objProductoLC = new DTO_ProductoListaCompra();
            if (!IsPostBack)
            {
                txtEstado.Text = "Creado";
                txtFechaE.Text = DateTime.Now.Date.ToShortDateString();
                llenarDDLProveedor();
                
                objProductoLC.idListaCompra = (int)Session["idListaC"];
                llenarDDLProducto(objProductoLC);

            }
        }

        public void llenarDDLProveedor()
        {
            CTR_Proveedor objProveedor = new CTR_Proveedor();
            DataSet dsProveedor = new DataSet();
            dsProveedor = objProveedor.SelectProveedor();
            ddlProveedor.DataTextField = "razonSocial";
            ddlProveedor.DataValueField = "idProveedor";
            ddlProveedor.DataSource = dsProveedor;
            ddlProveedor.DataBind();
            ddlProveedor.Items.Insert(0, "Seleccionar");
        }

        public void llenarDDLProducto(DTO_ProductoListaCompra DTOProducto)
        {
            
            CTR_ProductoListCompra objProducto = new CTR_ProductoListCompra();
            DataTable dtProducto = new DataTable();
            dtProducto = objProducto.SelectProductoLC(DTOProducto);
            ddlProducto.DataTextField = "nombreProducto";
            ddlProducto.DataValueField = "idProducto";
            ddlProducto.DataSource = dtProducto;
            ddlProducto.DataBind();
            ddlProducto.Items.Insert(0, "Seleccionar");

        }

        protected void btnAgregarPedido_Click(object sender, EventArgs e)
        {
            
        }
        protected void ddlProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCantidad.Text = "";
        }

        protected void ddlMedioPago_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnAgregarP_Click(object sender, EventArgs e)
        {
            DTO_Pedido DTOPedido = new DTO_Pedido();
            objProductoLC = new DTO_ProductoListaCompra();
            objProductoLC.idListaCompra = (int)Session["idListaC"];
            DTOPedido.FechaEmision = Convert.ToDateTime(DateTime.Now.Date.ToShortDateString());
            DTOPedido.FechaEntrega = Calendar1.SelectedDate;
            DTOPedido.modoPago = ddlMedioPago.SelectedValue;
            DTOPedido.idProveedor = int.Parse(ddlProveedor.SelectedValue);
            DTOPedido.idListaCompra = objProductoLC.idListaCompra;
            DTOPedido.Estado = 1;         
            CTR_Pedido objPedido = new CTR_Pedido();
            objPedido.InsertPedido(DTOPedido);
            txtCantidad.Text = "";
            //Insertar en DetallePedido

        }
        public void RegistarDetallePedido()
        { }
        public DTO_Producto ObtenerFormato(int idPd)
        {
            int idP = 0;
            DTO_Producto objProducto = new DTO_Producto();
            objProductoLC.idListaCompra = (int)Session["idListaC"];
            CTR_ProductoListCompra objProdLC = new CTR_ProductoListCompra();
            DataTable dtProducto = new DataTable();
            dtProducto = objProdLC.SelectProductoLC(objProductoLC);

            foreach(DataRow row in dtProducto.Rows)
            {
                idP = Convert.ToInt32(row["idProducto"].ToString());
                if (idP == idPd)
                {
                    objProducto.formato = row["formato"].ToString();
                    objProducto.precioCompra = Convert.ToDecimal(row["precioCompra"].ToString());
                } 
            }
            return objProducto;
        }

         
        public void ListarProducto()
        {
            DataTable dtProducto = new DataTable();
            if(dtProducto.Columns.Count==0)
            {
                dtProducto.Columns.Add("Producto", System.Type.GetType("System.String"));
                dtProducto.Columns.Add("Formato", System.Type.GetType("System.String"));
                dtProducto.Columns.Add("Cantidad", System.Type.GetType("System.Int32"));
                dtProducto.Columns.Add("Precio", System.Type.GetType("System.Decimal"));
                dtProducto.Columns.Add("Total", System.Type.GetType("System.Decimal"));
            }
            DataTable dt_T = new DataTable();
            dt_T = (DataTable)Session["dtTransformacion"];

            if (dt_T != null)
            {
                dtProducto = dt_T;
            }
            DataRow row = dtProducto.NewRow();
            row["Producto"] = ddlProducto.SelectedItem.Text;
            row["Formato"] = ObtenerFormato(Convert.ToInt32(ddlProducto.SelectedItem.Value)).formato;
            row["Cantidad"] = Convert.ToInt32(txtCantidad.Text);
            row["Precio"]= ObtenerFormato(Convert.ToInt32(ddlProducto.SelectedItem.Value)).precioCompra;
            row["Total"] = Convert.ToDecimal(row["Cantidad"]) * Convert.ToDecimal(row["Precio"]);

            dtProducto.Rows.Add(row);
            GridView1.DataSource = dtProducto;
            GridView1.DataBind();
            Session.Add("dtTransformacion", dtProducto);

        }
        public string DataFieldSelect(string idI)
        {
            string cadena = "";

            //foreach (ListItem item in ddlProducto.Items)
            //{
            //    item.Value = ddlProducto.SelectedItem.Value;
            //    if (item.Value==idI)
            //    cadena = string.Format("{0} <br/>", item.Text);

            //}

       
           
     

            return cadena;
        }

        protected void btnAñadirProducto_Click(object sender, EventArgs e)
        {
            ListarProducto();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionarPedido.aspx");
        }
    }
}