1. Télécharger le projet ici : https://github.com/bobbyblanchette/INF1453-ProjetFinal/archive/master.zip
2. La connexion à la base de données se fait automatiquement. Seulement appuyer sur 'Démarrer le débogage (F5)'
3. Voici la définition des fichiers et méthodes importantes de notre projet :




- /App_Data/Atlas.mdb									// La base de données



Views:
 - /Views/Home/Index.cshtml								// La page d'accueil du programme
 - /Views/Home/BookDetails.cshtml						// La page description d'un livre
 
 - /Views/Shared/DisplayTemplates/BookModel.cshtml		// Le template pour les objets de type BookModel
 - /Views/Shared/DisplayTemplates/Currency.cshtml		// Le template pour les objets avec l'attribut UIHint("Currency")
 - /Views/Shared/DisplayTemplates/Rating.cshtml			// Le template pour les objets avec l'attribut UIHint("Rating")
 - /Views/Shared/_Layout.cshtml							// Le conteneur pour toutes les pages
 - /Views/Shared/_LoginPartial.cshtml					// Le template pour les boutons de connexion et d'inscription
 
 - /Views/User/Library.cshtml							// La page de la librairie personnelle des utilisateurs
 - /Views/User/Login.cshtml								// La page de connexion des utilisateurs
 - /Views/User/Register.cshtml							// La page d'inscription pour les nouveaux utilisateurs
 
 
 
Models:			// La validation des modèles se fait selon les attributs assignés dans les modèles, ainsi que la custom validation (dans le cas de LoginModel et RegisterModel)
 - /Models/BookModel.cs									// Modèle de livre, contenant toutes les informations nécessaires
 - /Models/HomeModel.cs									// Modèle pour la page d'accueil et la librairie, contenant une liste de BookModels et une liste de Categories
 - /Models/LoginModel.cs								// Modèle pour la page de connexion, contenant des attributs de validation et la custom validation qui vérifie le mot de passe dans la base de données avec le système de salt/hash
 - /Models/RegisterModel.cs								// Modèle pour la page d'inscription, contenant des attributs de validation et la custom validation qui vérifie si le nom d'utilisateur est déjà pris
 
 
 
Controllers:
 - /Controllers/HomeController.cs						
				// Le contrôleur de base, pour toutes les opérations qui ne concernent pas les utilisateurs
				// La méthode Index va retourner la vue "Index.cshtml", avec un modèle HomeModel contenant les livres, filtrés selon les paramètres entrés (searchString et category)
				// La méthode BookDetails va retourner la vue "BookDetails.cshtml", avec un modèle BookModel contenant les détails du livre demandé dans les paramètres (id)
 - /Controllers/UserController.cs
				// Le contrôleur pour toutes les opérations concernant les utilisateurs
				// Les méthode Login() et Register() avec les attributs HttpGet vont tout simplement retourner les vues "Login.cshtml" ou "Register.cshtml", avec des modèles LoginModel ou RegisterModel vides
				// La méthode Library va retourner la vue "Library.cshtml", avec un modèle "HomeModel.cs" contenant les livres qui sont dans la librairie de l'utilisateur, filtrés selon les paramètres entrés (searchString et category)
				// La méthode DownloadBook va retourner le fichier PDF selon le lien contenu dans le modèle BookModel entré en paramètre
				// La méthode Login avec l'attribut HttpPost va vérifier la validité du modèle LoginModel entré en argument, et, si ce modèle est valide, utilises FormsAuthentication pour connecter l'utilisateur
				// La méthode Register avec l'attribut HttpPost va vérifier la validité du modèle RegisterModel entré en argument, et, si ce modèle est valide, va créer la requête pour inscrire l'utilisateur
				// La méthode Logout va tout simplement déconnecter l'utilisateur avec FormsAuthentication
				// La méthode ConfirmPurchase va créer la requête pour ajouter une entrée dans la table Sales de la base de données, reliant le ID de l'utilisateur au ID d'un livre
				// La méthode SaltAndHash va créer un salt, pour ensuite générer un hash avec la méthode generateHash et retourner un string contenant le salt et le hash, séparés par un "|"
				// La méthode CreateSalt va créer un salt aléatoire à l'aide de la méthode RNGCryptoServiceProvider (pour avoir un nombre aléatoire riche)

				
				
Autres:
 - /Utils.cs	
				// Cette classe contient uniquement des méthodes statiques, disponibles partout dans la solution
				// La méthode deserialize va créer un modèle BookModel et le populer selon une dataTable reçue en paramètre
				// La méthode generateHash va générer un hash à partir d'un password et un salt entrés en paramètres, avec 5000 itérations de SHA256, en réintroduisant le salt dans chaque itération
				// La méthode SHA256Hash va générer un hash SHA256 à partir d'une valeur entrée en paramètre
 
 - /Scripts/paypal/express_checkout.js					// Ce script est le script fourni par Paypal Express Checkout pour accéder à leur API
														// Le callback va rediriger l'utilisateur vers le lien pour confirmer l'achat (qui va aller dans UserController.cs --> ConfirmPurchase())

 - /Content/Site.css									// Le fichier CSS pour tout le site web
 
 - /Web.config											// Ce fichier contient le connectionString, qui va aller chercher la base de données dynamiquement dans le sous-dossier /App_Data/ du dossier de la solution
 
 
 