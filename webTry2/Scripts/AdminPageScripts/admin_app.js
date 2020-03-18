/* very similar to emloyee, try to use employye code
 * need to change employee code so the regular functions will be functions and now $scope.func = function
 * add sql query in AdminController to get ALL encryptors
 * add an request table so each updated employee want to do will be shown 
   on admin table, Edit window should stay the same just with "approve" button
 * 
 */

var app = angular.module("adminApp", ['commonMod']);
app.controller("adminPageContoller", ['$scope', '$location','$http','commonFunctions', function ($scope,$location,$http,commonFunctions) {

    var postData = $location.absUrl().split("?");
    $scope.userName = postData[1].split("=");
    $scope.userName = $scope.userName[1];

    //    for EDIT and Display
    /* introduction: userEncryptors including encryptors locations- lat and long!
     * markers init function: initMarkers without setting them on map
     * set Markers on map function: setMarkersOnMap
     */
    $scope.Encryptors = [];
    var markesrArray = [];
    $scope.gMap;

    $scope.statusArray = []; // need to have all posible status
    $scope.sitesLocationsArray = []; // have all locations buildings and coordinates
    //user basic deatails
    var userDetails;

    //variables for showing data on edit window
    $scope.tempDataForEditWindow;
    var tempDataIndex;






    /*              controller functions                   */
    /* on load - getting all encryptors
     * also get user details
     */
    $scope.onLoad = function () { //see on employee.js impementation
        userDetails = commonFunctions.getUserDetails($scope.userName);
        //getting all encryptors
        $http({
            method: "POST",
            //url define what function we apply to when post to the server
            url: "loadEmployeeEncryptors",
            data: { userName : ""}
        }).then(function (dataReturn) {
            var data = dataReturn.data;

            $.each(data, function (index, record) {
                $scope.Encryptors.push(record);
            });
            //loading map object
            $scope.initialize();
        });
        //getting user details  

    };

    //function for display and update 
    $scope.getRowData = function (index) {
        $scope.tempDataForEditWindow = $scope.Encryptors[index];
        //select initialize 
        switch ($scope.Encryptors[index].status) {
            case "in use":
                $('#statusSelect').val('1');
            case "destroyed":
                $('#statusSelect').val('2');
            case "lost":
                $('#statusSelect').val('3');
            case "delivered":
                $('#statusSelect').val('4');
            default:
                $('#statusSelect').val('1');
        }
        tempDataIndex = index;
    };

    //closeEdit: edit close-btn without saving! 
    $scope.closeEdit = function () {
        $scope.tempDataForEditWindow = $scope.Encryptors[tempDataIndex];
    };

    $scope.logout = function () {
        window.location.href = "/";
    };

    /*      map view page , second page for employee        */
    $scope.searchResults = function () {
        //continue with searching !
        //var searchIn = $('#searchInput').val(); 
        //var result = $scope.Encryptors.includes(searchIn);
        //Console.log(result);
    }

    $scope.initialize = function () {
        var googleMapOption = {
            zoom: 6.99,
            maxZoom: 17.43, 
            minZoom : 5.15,
            center: new google.maps.LatLng(32.3571742, 36.9767603),
            mapTypeId: google.maps.MapTypeId.TERRAIN,
            mapTypeControl: true,
            streetViewControl: false
        };

        $scope.gMap = new google.maps.Map(document.getElementById('googleMap'), googleMapOption);

        initMarkers();//init markers on map
        clusterMarkers();

    };

    /* introduction: init maps create markers on map while taking lat and lng from
     * userEncryptors object
     * also push markers to markesrArray
     */
    function initMarkers() {
        angular.forEach($scope.Encryptors, function (encryptor, index) {
            var marker = new google.maps.Marker({
                //setting marker position
                position: new google.maps.LatLng(encryptor.deviceLocation.latitude, encryptor.deviceLocation.longitude),
                map: $scope.gMap
            });
            markesrArray.push(marker);
        });
    };

    /* introduction: clusterMarkers function cluster marker by their location 
     * to see that you can search for markerClusterer on google
     */
    function clusterMarkers() {
        var markerCluster = new MarkerClusterer($scope.gMap, markesrArray,
            { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
    }


    function viewByClicked() {
        var x;
        switch ($('#viewBySelect').val()) {
            case '2': {
                $('#viewBySite').css("visibility", "visible");
                break;
            }
            default: {
                $('#viewBySite').css("visibility", "hidden");
                break;
            }

        }
        
    }


}]);