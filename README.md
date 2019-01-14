# Baufest | POC - Face API >> Cencosud

:octocat: En este repositorio vas a poder encontrar la POC de implementación de Face API realizada para el Workshop de 2019 para Cencosud. El principal objetivo es experimentar con el reconocimiento de imágenes, en este caso en particular, de rostros de personas a través de [Face API](https://azure.microsoft.com/en-us/services/cognitive-services/face/).

A continuación, se presentan los diferentes pasos que deberás realizar para poder finalizar la implementación parcial de la POC, mientras aprendés cómo trabajar con [Cognitive Services](https://azure.microsoft.com/en-us/try/cognitive-services/) de Microsoft Azure.

Una vez que finalices con los pasos descriptos vas a poder contar con una aplicación MVC integral y funcionando que permitirá generar grupos de personas, asociarles diferentes fotos, y luego poder realizar reconocimiento de personas dentro de cada grupo analizando nuevas imágenes de las personas que lo conforman.

Hecha esta breve introducción, pasemos a trabajar en la implementación de la POC.


## Tabla de Contenidos
- [Pre-requisitos](#pre-requisitos)
- [Validando la Subscription Key a Face API](#validando-la-subscription-key-a-face-api)
- [Manos a la Obra](#manos-a-la-obra)
  - [Paso 1: Setup de la Solución](#paso-1-setup-de-la-solución)
  - [Paso 2: Implementación del Análisis de Imágenes](#paso-2-implementación-del-análisis-de-imágenes)


## Pre-requisitos
:point_right: Antes de comenzar, es necesario que validemos algunos pocos requisitos previos:
- Tener instalado **Visual Studio 2017**, o alguna versión superior.
- Tener instalado **.Net Framework 4.6.2** en nuestro entorno de desarrollo.
- Para que la aplicación funcione correctamente es necesario contar con una Base de Datos. Si bien te recomendamos SQL Server, podés utilizar cualquier otro motor siempre y cuando modifiques la capa Repository de la solución.
  - Si vas a utilizar SQL Server, o SQL Azure, te recomendamos que instales [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017)
  - También podés utilizar un SQL Server Express Local. La solución por defecto provee un _Connection String_ a una base de datos local en la carpeta _App_Data_.
- Para poder realizar el análisis y reconocimiento de caras es necesario generar una _Subscription Key_ a Face API :key:.
  - Si tenés una cuenta activa en Azure, podés hacer esto directamente desde el [Portal Web](https://portal.azure.com/) creando un nuevo _resource_ del tipo **Face** en la categoría _AI + Machine Learning_.
  - Si no tenés una cuenta Azure activa, podés generar una _Subscription Key_ de prueba a través del siguiente [enlace](https://azure.microsoft.com/en-us/try/cognitive-services/).
- De forma opcional, te recomendamos que instales [Postman](https://www.getpostman.com/apps) (si aún no lo tenés disponible en tu entorno) para poder probar los endpoints de Face API y tener una forma de validar la _Subscription Key_ y el correcto funcionamiento del servicio.

Por último, te recomendamos tener a mano los siguientes links de **Face API** ya que te podrán ser útil en más de una oportunidad:
- [Página principal de Face API](https://azure.microsoft.com/en-us/services/cognitive-services/face/)
- [Página con la documentación, guías rápidas y tutoriales](https://docs.microsoft.com/en-us/azure/cognitive-services/face/)
- [Página con las referencias de la API](https://westus.dev.cognitive.microsoft.com/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395236)


## Validando la Subscription Key a Face API
Para quedarnos más tranquilos respecto de que Face API se encuentra correctamente configurado, vamos a validar el Endpoint y la Key analizando una imagen arbitraria:
1. Encontrar en tu buscador favorito una imagen cualquiera con la cara de una persona y copiar la URL de dicha imagen (ojo que no todas las imágenes que están en internet permiten que se accedan desde fuera de un browser, si ves que el servicio retorna error, por favor probá con otra imagen para descartar esta opción).
2. Ejecutar Postman
3. Crear una nueva POST request con la siguiente configuración:
   - _URL_: La provee Face API como parte de la configuración cuando se generó la Subscription Key. 
     - Es del estilo: `https://[LOCATION].api.cognitive.microsoft.com/face/v1.0`
     - En donde [LOCATION] es la locación en donde registraste el servicio de Face API
     - Para poder probar el endpoint, deberemos agregarle al final de la ruta el siguiente parámetro: `/detect?returnFaceId=true`
   - _Headers_: Agregar dos headers similares a los que se presentan a continuación, y reemplazar el tag [SUBSCRIPTION_KEY] por el valor provisto por Face API:
     - `Content-Type: application/json`
     - `Ocp-Apim-Subscription-Key: [SUBSCRIPTION_KEY]`
   - _Body_: Completar de la siguiente manera: `{
    "url": "[URL_PHOTO]"
}` y reemplazar [URL_PHOTO] con la URL de la foto que elegimos en el primer punto

Si la _Subscription Key_ fue generada correctamente y el endpoint configurado de forma satisfactoria, al ejecutar el request se debería retornar una respuesta similar a la siguiente :ok_hand::
```json
  [    
    {
      "faceId": "1179755f-6c41-46ba-9a9a-cbcd2d0a70d2",
      "faceRectangle": {
        "top": 54,
        "left": 128,
        "width": 99,
        "height": 99
      }
    }
  ]
```


## Manos a la Obra
¡¡Ya es tiempo de ponernos a codear!! :clap: :clap:


### Paso 1: Setup de la Solución
:open_hands: Pero antes de tirar la primera línea de código, terminemos de realizar los siguientes ajustes:
1. Clonar la solución del repositorio en nuestro entorno local
2. Hacer un Build de la solución
   - Si todo sale bien, NuGet debería bajarte todos los componentes necesarios de forma automática
   - Si no se bajan de forma automática, revisá el siguiente [Knowledge Package](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore-troubleshooting) para obtener más información
   - Sólo a título informativo, para la realización de la POC, vamos a utilizar los siguientes componentes de [Microsoft Project Oxford](https://github.com/Microsoft/Cognitive-Face-Windows):
     - Microsoft.ProjectOxford.Common
     - Microsoft.ProjectOxford.Face
3. Tomate unos minutos para analizar la arquitectura de la solución y los diferentes proyectos que forman parte de ella.
4. Modificar el archivo `Settings.config` en el proyecto `BF.POC.FaceAPI.Web` para incluir la configuración de Face API (los datos son los mismos que utilizaste previamente)
   - Agregar la URL de Face API (`https://[LOCATION].api.cognitive.microsoft.com/face/v1.0`)
   - Agregar la Key de la suscripción de Face API (`[SUBSCRIPTION_KEY]`)
5. Instanciar el objeto `FaceServiceClient` en el constructor de la clase `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` utilizando la sobrecarga que permite indidcar tanto la URL como la Key de la suscripción de Face API:
   - Ya que esto nos permitirá invocar correctamente a la API desde nuestra aplicación
   - La clase `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` es un _facade_ propio que nos abstraerá de la utilización de Microsoft.ProjectOxford.
6. Ejecutar la aplicación y verificar que la página principal se muestra correctamente
   - La solución utiliza _Code First_, con lo cual no debemos preocuparnos de generar la Base de Datos a mano, sino que esto se hará de forma automática
   - Si durante la ejecución del ejercicio modificás el _Modelo de Objetos_ o la Base de Datos se corrompe, la misma se podrá borrar de forma segura y se volverá a regenerar al ejecutar nuevamente la aplicación
7. Navegar las diferentes páginas de la aplicación (Groups, People, Faces, Test) para validar que lsa mismas se muestren correctamente
   - _Por favor, aún no des de alta registros a través de los ABMs, ya que la implementación está incompleta y podrían quedar datos basura en la Base de Datos que nos compliquen la implementación posterior_


### Paso 2: Implementación del Análisis de Imágenes
De la misma forma que hicimos con Postman previamente, vamos a realizar una implementación auxiliar que nos va a servir para validar el correcto funcionamiento e integración de Face API con la Solución, y por qué no, permitirnos jugar con la API pasándole diferentes imágenes a analizar y pudiendo ver cómo funciona la lógica de detección de emociones.

Esta funcionalidad auxiliar se accede desde el Menú **Test** disponible en la aplicación web y si bien no es necesaria para poder realizar el reconocimiento de personas a través de sus rostros, es útil para probar nuestra integración.

_A los fines prácticos de la POC, nos vamos a centrar casi únicamente en la integración con Face API, por lo que no haremos mucho foco en otras capas de la arquitectura de la aplicación, las cuales podrás analizar por tu cuenta para aprender más sobre la misma._
_Es por ello que todo lo que comprende tanto el frontend web como el acceso a datos ya se encuentra construido y a priori no se deberán realizar modificaciones._

Para poder realizar la implementación, se deberán seguir los pasos detallados a continuación:
1. En el _action_ `Analyze` del _controller_ `TestController` ya se está invocando al _method_ `faceAPIClient.FaceDetectAsync()` pasándole la imagen a analizar recibida desde browser.
2. Navegar en la solución a la implementación del _method_ `FaceDetectAsync` en la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` y realizar la siguiente implementación:
   - Invocar el _method_ `DetectAsync` de la _property_ `faceServiceClient`
   - Utilizar la firma que contempla un objeto de tipo `Stream`
   - Indicar por parámetro que sí se desea obtener el `FaceId` en la respuesta
   - Indicar por parámetro que no se desean obtener los _face landmarks_ en la respuesta
   - Proporcionar los _face attributes_ a analizar pasando como cuarto parámetro la propiedad `faceAttributes` ya definida en la clase.
3. Ejecutar la aplicación, navegar a la pantalla Test y realizar diferentes pruebas para validar la correcta integración con la API.

