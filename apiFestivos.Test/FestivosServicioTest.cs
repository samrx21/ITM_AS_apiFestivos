using apiFestivos.Aplicacion.Servicios;
using apiFestivos.Core.Interfaces.Repositorios;
using apiFestivos.Core.Interfaces.Servicios;
using Moq;
using apiFestivos.Dominio.Entidades;
using apiFestivos.Dominio.DTOs;


namespace apiFestivos.Test
{
    public class FestivosServicioTest
    {
        private readonly Mock<IFestivoRepositorio> mockRepositorio;
        private readonly IFestivoServicio servicio;

        public FestivosServicioTest()
        {
            mockRepositorio = new Mock<IFestivoRepositorio>();
            servicio = new FestivoServicio(mockRepositorio.Object);
        }

        // Punto 1

        [Fact]
        public async void EsFestivo_DeberiaDevolverTrue_CuandoLaFechaEsFestiva()
        {
            // Arrange: Simulamos una lista de festivos que incluye la fecha 25 de diciembre de 2024.
            var fechaFestiva = new DateTime(2024, 12, 25); // Ejemplo de Navidad.
            var festivos = new List<Festivo>
            {
                new Festivo { Id = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1, DiasPascua = 0, IdTipo = 1 },
                new Festivo { Id = 2, Nombre = "Día del Trabajo", Dia = 1, Mes = 5,  DiasPascua = 0, IdTipo = 1},
                new Festivo { Id = 3, Nombre = "Día de la Independencia", Dia = 20, Mes = 7 , DiasPascua = 0, IdTipo = 1 },
                new Festivo { Id = 4, Nombre = "Batalla de Boyacá", Dia = 7, Mes = 8 ,  DiasPascua = 0, IdTipo = 1},
                new Festivo {Id = 5, Nombre = "Día de la Raza", Dia = 12, Mes = 10, DiasPascua = 0, IdTipo = 1},
                new Festivo {Id = 6, Nombre = "Navidad", Dia = 25, Mes = 12, DiasPascua = 0, IdTipo = 1}
            };

            // Configuramos el mock para devolver esta lista de festivos.
            mockRepositorio.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            // Act: Llamamos al método EsFestivo con una fecha que es festiva.
            var resultado = await servicio.EsFestivo(fechaFestiva);

            // Assert: Verificamos que el resultado sea true.
            Assert.True(resultado);
        }

        [Fact]
        public async void EsFestivo_DeberiaDevolverFalse_CuandoLaFechaNoEsFestiva()
        {
            // Arrange
            var fechaNoFestiva = new DateTime(2024, 12, 26); // Una fecha que no es festiva
            var festivos = new List<Festivo>
            {
                new Festivo { Id = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1, DiasPascua = 0, IdTipo = 1 },
                new Festivo { Id = 2, Nombre = "Día del Trabajo", Dia = 1, Mes = 5,  DiasPascua = 0, IdTipo = 1},
                new Festivo { Id = 3, Nombre = "Día de la Independencia", Dia = 20, Mes = 7 , DiasPascua = 0, IdTipo = 1 },
                new Festivo { Id = 4, Nombre = "Batalla de Boyacá", Dia = 7, Mes = 8 ,  DiasPascua = 0, IdTipo = 1},
                new Festivo {Id = 5, Nombre = "Día de la Raza", Dia = 12, Mes = 10, DiasPascua = 0, IdTipo = 1},
                new Festivo {Id = 6, Nombre = "Navidad", Dia = 25, Mes = 12, DiasPascua = 0, IdTipo = 1}
            };


            // Configuramos el mock para devolver esta lista de festivos.
            mockRepositorio.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            // Act: Llamamos al método EsFestivo con una fecha que no es festiva.
            var resultado = await servicio.EsFestivo(fechaNoFestiva);

            // Assert: Verificamos que el resultado sea false.
            Assert.False(resultado);
        }


        // Punto 2

        [Fact]
        public async Task ObtenerAño_DeberiaRetornarFechaCorrecta_CuandoEsFestivoTipo1()
        {
            // Arrange: Configurar un festivo de tipo 1 (fecha fija, como Año Nuevo)
            var año = 2024;
            var festivos = new List<Festivo>
            {
                new Festivo { Id = 1, Nombre = "Año Nuevo", Dia = 1, Mes = 1, DiasPascua = 0, IdTipo = 1 }
            };
            mockRepositorio.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            // Act: Llamamos al método ObtenerAño para verificar los festivos del año especificado
            var resultado = await servicio.ObtenerAño(año);

            // Assert: Comprobar que el festivo retornado tiene la fecha esperada
            var festivoResultado = Assert.Single(resultado); // Asegura que solo hay un festivo
            Assert.Equal(new DateTime(año, 1, 1), festivoResultado.Fecha); // Año Nuevo es el 1 de 
        }



        [Fact]
        public async Task ObtenerAño_DeberiaRetornarSiguienteLunes_CuandoEsFestivoTipo2()
        {
            // Arrange: Configurar un festivo de tipo 2 (movible, como el Día de San José que cae en lunes)
            var año = 2024;
            var festivos = new List<Festivo>
             {
                 new Festivo { Id = 2, Nombre = "Día de San José", Dia = 19, Mes = 3, DiasPascua = 0, IdTipo = 2 }
             };
            mockRepositorio.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await servicio.ObtenerAño(año);

            // Assert: El Día de San José en 2024 es 19 de marzo (martes) y debería moverse al 25 de marzo (lunes)
            var festivoResultado = Assert.Single(resultado);
            Assert.Equal(new DateTime(año, 3, 25), festivoResultado.Fecha);
        }


        [Fact]
        public async Task ObtenerAño_DeberiaRetornarSiguienteLunes_CuandoEsFestivoTipo4()
        {
            // Arrange: Configurar un festivo de tipo 4 (como Lunes de Pascua, relativo a Semana Santa)
            var año = 2025;
            var festivos = new List<Festivo>
            {
                new Festivo { Id = 3, Nombre = "Lunes de Pascua", Dia = 0, Mes = 0, DiasPascua = 1, IdTipo = 4 }
            };
            mockRepositorio.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await servicio.ObtenerAño(año);

            // Assert: En 2025, Semana Santa empieza el 13 de abril, por lo que el lunes siguiente es 14 de abril
            var festivoResultado = Assert.Single(resultado);
            Assert.Equal(new DateTime(año, 4, 14), festivoResultado.Fecha);
        }


    }
}
