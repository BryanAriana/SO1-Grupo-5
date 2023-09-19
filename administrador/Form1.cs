using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace administrador
{
    public partial class Form1 : Form
    {
        private Process selectedProcess; // Almacena el proceso seleccionado
        private PerformanceCounter cpuCounter; // Contador de uso de CPU
        //private int selectedProcessId = -1;
        public Form1()
        {
            InitializeComponent();
      
            cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);

        }

        // Metodo que se ejecuta al cargar el formulario
        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshProcessList(); // Actualiza la lista de procesos
            timer1.Start(); // Inicia el temporizador para actualización automática
        }

        // Metodo para actualizar la lista de procesos en el DataGridView
        private void RefreshProcessList()
        {
            Process[] processes = Process.GetProcesses();
            dataGridView1.Rows.Clear(); // Limpia las filas existentes en el DataGridView

            foreach (var process in processes)
            {
                try
                {
                    // Obtiene el uso de CPU y la memoria del proceso
                    var cpuUsage = GetCpuUsage(process);
                    var memoryUsage = GetMemoryUsage(process);

                    // Agrega una fila con información del proceso al DataGridView
                    dataGridView1.Rows.Add(
                        process.ProcessName,
                        process.Id.ToString(),
                        cpuUsage.ToString("0.00") + "%",
                        memoryUsage.ToString("0.00") + "MB"
                    );
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error al obtener el uso de CPU: " + ex.Message);
                }
            }
        }

        // Metodo para obtner el uso de memoria de un proceso en mb
        private double GetMemoryUsage(Process process)
        {
            return process.WorkingSet64 / (1024.0 * 1024.0); // Convierte a megabytes
        }

        // Metodo para obtener el uso de CPU de un proceso en porcentaje
        private double GetCpuUsage(Process process)
        {
           
            var startTime = DateTime.UtcNow.AddMilliseconds(-process.TotalProcessorTime.TotalMilliseconds);
            var currentTime = DateTime.UtcNow;
            var cpuUsage = 100.0 * (process.TotalProcessorTime.TotalMilliseconds / (currentTime - startTime).TotalMilliseconds);

            return cpuUsage;
        }

        // Boton para detener un proceso
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                var processId = int.Parse(selectedRow.Cells[1].Value.ToString());

                try
                {
                    // Obtiene el proceso seleccionado y lo detiene
                    Process process = Process.GetProcessById(processId);
                    process.Kill();
                    RefreshProcessList(); //Actualiza la lista de procesos
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error al detener el proceso: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Selecciona un proceso para detenerlo.");
            }
        }

        //  Botón para actualizar la lista de procesos
        private void button1_Click(object sender, EventArgs e)
        {
            {
                RefreshProcessList();
            }
        }

        // Metodo del temporizador para actualización automática
        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshProcessList(); // Actualiza la lista de procesos
            if (selectedProcess != null) // Actualiza el uso de CPU del proceso seleccionado
            {
                try
                {
                    
                    var cpuUsage = GetCpuUsage(selectedProcess);
                   
                }
                catch
                {
                    
                 //   progressBar1.Value = 0;
                   
                }
            }
            else
            {
                
               // progressBar1.Value = 0;
                
            }
        }

        // Metodo para selección de una fila en el DataGridView
        private void dataGridView1_SelectionChange(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                var processId = int.Parse(selectedRow.Cells[1].Value.ToString());
                selectedProcess = Process.GetProcessById(processId);

                //textBox1.Text = $"ID: {selectedProcess.Id}, Nombre: {selectedProcess.ProcessName}";
            }
            else
            {
               
                selectedProcess = null;
                //textBox1.Text = String.Empty;
            }
        }

        // Botón para salir de la aplicación
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
