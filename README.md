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
  - [Paso 3: Implementación de la Gestión de Grupos](#paso-3-implementación-de-la-gestión-de-grupos)
  - [Paso 4: Implementación de la Gestión de Personas](#paso-4-implementación-de-la-gestión-de-personas)
  - [Paso 5: Implementación de la Gestión de Caras](#paso-5-implementación-de-la-gestión-de-caras)
  - [Paso 6: Implementación del Reconocimiento de Rostros](#paso-6-implementación-del-reconocimiento-de-rostros)


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

#### ¿Necesitás ayuda?
No te preocupes, a continuación te proporcionamos el código a implementar para que puedas validar tu solución:

<blockquote>
  <details>
    <summary> :no_entry: Solución :no_entry: </summary>
  <details>
    <summary>BF.POC.FaceAPI.Business.Clients.FaceAPIClient</summary>

  ```csharp
        public FaceAPIClient()
        {
            var subscriptionKey = ConfigurationManager.AppSettings["FaceApiSubscriptionKey"];
            var endpoint = ConfigurationManager.AppSettings["FaceApiEndpoint"];

            faceServiceClient = new FaceServiceClient(subscriptionKey, endpoint);
        }
  ```
  </details>
  </details>
</blockquote>


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

#### ¿Necesitás ayuda?
No te preocupes, a continuación te proporcionamos el código a implementar para que puedas validar tu solución:

<blockquote>
  <details>
    <summary> :no_entry: Solución :no_entry: </summary>
  <details>
    <summary>BF.POC.FaceAPI.Business.Clients.FaceAPIClient</summary>

  ```csharp
        public async Task<Face[]> FaceDetectAsync(byte[] image)
        {
            var stream = new MemoryStream(image);

            return await faceServiceClient.DetectAsync(stream, true, false, faceAttributes);
        }
  ```
  </details>
  </details>
</blockquote>


### Paso 3: Implementación de la Gestión de Grupos
Los grupos son las entidades que nos permitirán agrupar a diferentes personas bajo una misma temática para luego poder realizar el reconocimiento facial. Podés tener tantos grupos como desees, y tantas personas en cada uno de ellos como necesites. El punto es tenerlos segmentados para optimizar el _matching_ de los algoritmos de **Cognitive Services**.

Es por ello que lo primero que vamos a realizar es finalizar la implementación de los Grupos en nuestra aplicación.

_Si bien los Grupos se crean en la API y se gestionan desde allí, también estaremos guardando esta información en nuestra Base de Datos. Tené en cuenta esto a la hora de completar la funcionalidad correspondiente._

_Por otro lado, para el taller vamos a utilizar `Persons Groups` y no `Large Persons Groups`. lo mismo va a aplicar para el resto de los componentes. Por favor, tené en cuenta esto a la hora de realizar la implementación de la lógica en la aplicación._

Para poder realizar la implementación, se deberán seguir los pasos detallados a continuación:
1. En la _class_ `BF.POC.FaceAPI.Business.GroupManager` implementar los _methods_ `AddAsync()` y `UpdateAsync()`
   - Dentro de ellos invocar a los métodos de la _property_ `faceServiceClient` correspondientes (_ojo! son los métodos de nuestro cliente y no de Microsoft.ProjectOxford; recordá que la integración se realiza de la siguiente forma: `UI <<->> Manager <<->> Client <<->> Microsoft.ProjectOxford <<->> Face API`_).
   - En este punto debemos considerar que no podemos crear dos grupos con el mismo código en Face API, por eso en el _method_ `AddAsync` debemos validar que ya no exista previamente. En el caso del _method_ `UpdateAsync` no ejecutamos esta validación ya que desde la pantalla se limita la posibilidad de modificar este valor (_en un contexto real, no obstante, recomendamos ampliamente profundizar estas validaciones en ambos métodos a los fines de evitar eventuales ataques de usuarios malintencionados_).
2. En la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar los _methods_ `GroupCreateAsync()` y `GroupUpdateAsync()`
2. También en la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar el _method_ `GroupExistsAsync()`
   - De esta forma vamos a poder validar si el código del grupo ya se encuentra registrado en Face API o no.
   - Si la consulta a Face API retorna un valor distinto a `null`, entonces el código de grupo ya está registrado y no lo podremos usar.
   - Si el grupo no existe, la consulta retornará una `FaceAPIException` con el _code_ `PersonGroupNotFound`.
4. Ejecutar la aplicación, navegar a la pantalla Groups y realizar diferentes pruebas para validar la correcta integración con la API y el repositorio. Si se desea, también se pueden ejecutar _requests_ a Face API mediante Postman para validar que los datos se están registrando correctamente.

#### ¿Necesitás ayuda?
No te preocupes, a continuación te proporcionamos el código a implementar para que puedas validar tu solución:

<blockquote>
  <details>
  <summary> :no_entry: Solución :no_entry: </summary>
  <details>
    <summary>BF.POC.FaceAPI.Business.GroupManager</summary>

  ```csharp
        public async Task AddAsync(Group group)
        {
            if (!(await faceAPIClient.GroupExistsAsync(group.Code)))
            {
                await faceAPIClient.GroupCreateAsync(group.Code, group.Name);

                await groupRepository.AddAsync(group);
            }
            else
            {
                throw new BusinessException($"Person group '{group.Code}' already exists.");
            }
        }

        public async Task UpdateAsync(Group group)
        {
            await faceAPIClient.GroupUpdateAsync(group.Code, group.Name);

            await groupRepository.UpdateAsync(group);
        }
  ```
  </details>
  <details>
    <summary>BF.POC.FaceAPI.Business.Clients.FaceAPIClient</summary>

  ```csharp
        public async Task<bool> GroupExistsAsync(string code)
        {
            try
            {
                return await faceServiceClient.GetPersonGroupAsync(code) != null;
            }
            catch (FaceAPIException ex)
            {
                if (ex.ErrorCode == "PersonGroupNotFound")
                {
                    return false;
                }
                throw;
            }
        }

        public async Task GroupCreateAsync(string code, string name)
        {
            await faceServiceClient.CreatePersonGroupAsync(code, name);
        }

        public async Task GroupUpdateAsync(string code, string name)
        {
            await faceServiceClient.UpdatePersonGroupAsync(code, name);
        }
  ```
  </details>
  </details>
</blockquote>


### Paso 4: Implementación de la Gestión de Personas
Las Personas son aquellas entidades que nos van a permitir registrar los datos de las personas en un grupo, y contener las diferentes Caras (o Faces) que registraremos en el siguiente paso.

_De la misma forma que con los grupos, vamos a estar registrando los datos tanto en Face API como en nuestro repositorio a fines de poder manejar luego las imágenes asociadas a las caras de cada persona._

Para poder realizar la implementación, se deberán seguir los pasos detallados a continuación:
1. En la _class_ `BF.POC.FaceAPI.Business.PersonManager` implementar los _methods_ `AddAsync()` y `UpdateAsync()`
   - Dentro de ellos invocar a los métodos de la _property_ `faceServiceClient` correspondientes
   - Al pasar los parámetros, tené en cuenta que el código del grupo lo vas a necesitar obtener previamente, y el resto de los parámetros lo vas a poder obtener la entidad `person`.
   - En el _method_ `AddAsync()` no te olvides de asignar el PersonId obtenido de Face API a la _property_ correspondiente antes de invocar al repositorio para guardar los datos.
   - Implementar en ambos _methods_ las llamadas al repositorio de personas.
2. En la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar los _methods_ `PersonCreateAsync()` y `PersonUpdateAsync()` llamando a los métodos correspondientes de la API de Face. Si tenés dudas respecto qué métodos invocar, podés consultar la documentación proporcionada más arriba en este archivo.
3. Ejecutar la aplicación, navegar a la pantalla People y realizar diferentes pruebas para validar la correcta integración con la API y el repositorio. La forma más rápida de validar que la misma se ha realizado correctamente es verificando que una vez dada de alta la persona, la columna _Person ID_ muestre el _Guid_ proporcionado por la API de Face. Otra alternativa es ejecutar _requests_ a Face API mediante Postman para validar que los datos se están registrando correctamente.

#### ¿Necesitás ayuda?
No te preocupes, a continuación te proporcionamos el código a implementar para que puedas validar tu solución:

<blockquote>
  <details>
  <summary> :no_entry: Solución :no_entry: </summary>
  <details>
    <summary>BF.POC.FaceAPI.Business.PersonManager</summary>

  ```csharp
        public async Task AddAsync(Person person)
        {
            var group = groupRepository.GetById(person.GroupId);

            person.APIPersonId = await faceAPIClient.PersonCreateAsync(group.Code, person.Fullname);

            await personRepository.AddAsync(person);
        }

        public async Task UpdateAsync(Person person)
        {
            var group = groupRepository.GetById(person.GroupId);

            await faceAPIClient.PersonUpdateAsync(group.Code, person.APIPersonId, person.Fullname);

            await personRepository.UpdateAsync(person);
        }
  ```
  </details>
  <details>
    <summary>BF.POC.FaceAPI.Business.Clients.FaceAPIClient</summary>

  ```csharp
        public async Task<Guid> PersonCreateAsync(string groupCode, string personName)
        {
            return (await faceServiceClient.CreatePersonInPersonGroupAsync(groupCode, personName)).PersonId;
        }

        public async Task PersonUpdateAsync(string groupCode, Guid personId, string personName)
        {
            await faceServiceClient.UpdatePersonInPersonGroupAsync(groupCode, personId, personName);
        }
  ```
  </details>
  </details>
</blockquote>


### Paso 5: Implementación de la Gestión de Caras
Llegamos al último paso necesario antes de poder implementar el reconocimiento de rostros en nuestra aplicación. En este punto vamos a trabajar la implementación de la registración de caras a través de diferentes imágenes para cada una de las personas de un grupo dado.

_Es importante que consideremos que, al registrar caras para una persona, utilicemos imágenes donde figure una única cara, sino el algoritmo no va a saber qué rostro es el que corresponde a la persona. No obstante, en la POC vamos a controlar esto, validando que exista una única persona en la imagen proporcionada.

Para poder realizar la implementación, se deberán seguir los pasos detallados a continuación:
1. En la _class_ `BF.POC.FaceAPI.Business.FaceManager` implementar el _method_ `AddAsync()`
   - Implementar la llamada al _method_ correspondiente de nuestra _Facade_ para agregar la imagen a Face API
   - No te olvides de asignar el FaceId obtenido de Face API a la _property_ `face.APIFaceId` antes de invocar al repositorio para guardar los datos.
   - Implementar la llamada al _method_ `AddAsync` del `FaceRepository` para guardar los datos de la imagen y su FaceId correspondiente
   - Implementar la llamada al _method_ `GroupTrainAsync()` de nuestra _Facade_ para que le indique a Face API que se reentrene para el grupo indicado a fines de poder realizar luego el reconocimiento de rostros.
2. En la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar el _method_ `FaceCountFacesAsync()`
   - Este método es necesario para poder validar en la imagen proporcionada cuántas caras existen.
   - Realizar la implementación de forma similar a como habíamos hecho previamente en la implementación de la página Test
   - Pero esta vez, en vez de pasar como cuarto parámetro la lista completa de _Face Attributes_, pasemos sólo `new FaceAttributeType[] { }` a fines de no sobrecargar a la API con información que luego no vamos a utilizar.
   - Utilizar la firma que contempla un objeto de tipo `Stream`
   - Retornar la lista de Faces que devuelve la API para que sea utilizada por el _Manager_ y determinar si la imagen es válida o no
3. En la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar el _method_ `FaceAddAsync()`
   - A través de este método vamos a dar de alta la cara de la persona en la API de Face
   - Para ello, deberemos invocar al _method_ correspondiente de Face API
   - Utilizar la firma que contempla un objeto de tipo `Stream`
   - Y retornar el valor `PersistedFaceId` retornado por la API para que pueda ser utilizado por el _Manager_
4. En la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar el _method_ `GroupTrainAsync()`
   - Una vez que se ha agregado una nueva cara a alguna de las personas del grupo, es necesario entrenar a Face API para que pueda realizar el reconocimiento de rostros. Para ello, sólo deberemos invocar al _method_ que permite entrenar un rupo de personas, y la API realizará todo el trabajo por nosotros.
5. Ejecutar la aplicación, navegar a la pantalla Faces y realizar diferentes pruebas para validar la correcta integración con la API y el repositorio. La forma más rápida de validar que la misma se ha realizado correctamente es verificando que una vez dada de alta la cara, la columna _Face ID_ muestre el _Guid_ proporcionado por la API de Face. Otra alternativa es ejecutar _requests_ a Face API mediante Postman para validar que los datos se están registrando correctamente.


### Paso 6: Implementación del Reconocimiento de Rostros
Es momento entonces de avanzar con el reconocimiento de rostros :metal:

Este es el último tramo de la POC, y consistirá en utilizar todo lo que ya hemos creado, más algunos métodos a finalizar su implementación, para poder realizar reconocimiento de personas en imágenes.

Para poder realizar la implementación, se deberán seguir los pasos detallados a continuación:
1. En la _class_ `BF.POC.FaceAPI.Business.GroupManager` implementar el _method_ `SearchCandidatesAsync()`
   - Primeramente, deberemos invocar al _method_ `FaceDetectAsync()` para obtener los diferentes rostros presentes en la imagen proporcionada y guardar este valor en una variable llamada `faces` del tipo `Microsoft.ProjectOxford.Face.Contract.Face[]`.
   - Luego se genera la lista de Candidatos, que va a contener la información reconocida por `FaceDetectAsync()` junto a la persona reconocida, en caso de que haya una coincidencia.
   - También se deberá generar un _array_ con los FaceIds obtenidos ya que nos será útil en el siguiente paso.
   - Deberemos invocar _method_ `FaceIdentifyFacesAsync()` para realizar el análisis de las personas y guardar el valor retornado en una variable llamada `identifyResult` del tipo `Microsoft.ProjectOxford.Face.Contract.IdentifyResult[]`.
   - Luego se deberá itera los resultados obtenidos, y se analizará si para cada registro existe o no un candidato retornado por Face API.
   - Si existe un Candidato, significa que hubo una coincidencia y Face API retorna el nivel de certidumbre y el _PersonId_ asociado a la persona.
   - Con el _PesonId_ deberemos buscar los datos de la persona registrada en nuestra Base de Datos.
   - Y guardaremos el _Confidence Level_ en la propiedad `Confidence` del objeto `candidates[i]` para mostrarlo luego por pantalla.
2. En la _class_ `BF.POC.FaceAPI.Business.Clients.FaceAPIClient` implementar el _method_ `FaceIdentifyFacesAsync()`
   - A través de este método se realizará la detección de rostros utilizando Face API
   - Para ello, deberemos invocar al _method_ correspondiente de la API con la siguiente configuración:
     - Emplear la firma del método que utiliza un objeto de tipo `Guid[]` para pasar los FaceId
     - Dado que no estamos utilizando un _Large Person Group_, este valor lo vamos a tomar como `null`
     - Como valor de corte para determinar si reconoce como válida a una persona o no, vamos a uilizar 65%
     - Vamos a trabajar con un solo candidato retornado por cada FaceId
   - Retornar la respuesta de la API para que pueda ser utilizada por el _Manager_ de Grupos
3. Ejecutar la aplicación, navegar a la pantalla Groups y seleccionar la opción _Search for a Person_ y realizar diferentes pruebas para validar la correcta integración con la API y el algoritmo de reconocimiento. En esta pantalla, sí se puede subir una imagen con varias personas y ver si reconoce a las que hayamos registrado previamente o no.


