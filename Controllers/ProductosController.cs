using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Tienda.Models;

namespace Tienda.Controllers
{
    public class ProductosController : Controller
    {
        private readonly string _connectionString;

        public ProductosController(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");



        //El codigo de abajo sirve para mostrar una lista de productos
        public IActionResult Index()
        {

            var productos = new List<Productos>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("ObtenerProductos", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    productos.Add(new Productos
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Descripcion = reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Stock = reader.GetInt32(4)
                    });
                }
            }

            return View(productos);

        }

        //Aqui se crea un nuevo producto
        public IActionResult Crear()
        {
            return View();
        }

        // aqui para guardar el producto
        [HttpPost]
        public IActionResult Crear(Productos producto)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("InsertarProducto", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(producto);
        }

        // aqui se logra editar productos
        public IActionResult Editar(int id)
        {
            var producto = new Productos();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Productos WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    producto.Id = reader.GetInt32(0);
                    producto.Nombre = reader.GetString(1);
                    producto.Descripcion = reader.GetString(2);
                    producto.Precio = reader.GetDecimal(3);
                    producto.Stock = reader.GetInt32(4);
                }
            }

            return View(producto);
        }

        // Actualizar un producto
        [HttpPost]
        public IActionResult Editar(Productos producto)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("ActualizarProducto", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", producto.Id);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(producto);
        }

        // Eliminar un producto
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("EliminarProducto", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }


}

