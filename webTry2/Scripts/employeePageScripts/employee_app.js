
var app = angular.module("employeeAng", []);
app.controller("employeePageContoller", function ($scope, $http, $location) {

    var postData = $location.$$absUrl.split("?");
    $scope.userName = postData[1].split("=");
    $scope.userName = $scope.userName[1];
    $scope.deliverToCustomerDetails = true;
    //    for EDIT and Display
    /* introduction: userEncryptors including encryptors locations- lat and long!
     * markers init function: initMarkers without setting them on map
     * set Markers on map function: setMarkersOnMap
     */
    $scope.userEncryptors = [];
    var markesrArray = [];
    $scope.gMap;

    $scope.statusArray = []; // need to have all posible status
    $scope.sitesLocationsArray = []; // have all locations buildings and coordinates
    //user basic deatails
    $scope.UserObj = {
        "UserName": "",
        "firstName": "",
        "lastName": "",
        "email": "",
        "phoneNumber": ""
    };

    //variables for showing data on edit window
    $scope.tempDataForEditWindow;
    var tempDataIndex;

    /*              controller functions                   */

    /* on load - getting all user encryptors by his user number - userName
     * also get user details */
    $scope.onLoad = function () {
        //getting encryptors
        $http({
            method: "POST",
            data: { "userName": $scope.userName },
            //url define what function we apply to when post to the server
            //url: "loadEmployeeEncryptors"
            url: "loadEmployeeEncryptors"
        }).then(function (dataReturn) {
            var data = dataReturn.data;

            $.each(data, function (index, record) {
                $scope.userEncryptors.push(record);
            });
             //loading map object
            $scope.initialize();
           
           // $('selectStatus option[value=' + $scope.userEncryptors + "']").attr("selected", "selected");
        });
        //getting user details  
    };

    //function for display and update 
    $scope.getRowData = function (index) {
        $scope.tempDataForEditWindow = $scope.userEncryptors[index];
        //select initialize 
        switch ($scope.userEncryptors[index].status) {
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

    $scope.logout = function () {
       window.location.href = "/";
    };

    $scope.ReportAbout=function(reportReason){
        var reason = $scope.SelectesReasonReport;
        switch (reason) {
            case 'changing encryptor location':
                $scope.deliverToCustomerDetails = false;
                break;
            case 'changing encryptor status':
                $scope.deliverToCustomerDetails = true;
                break;
            case 'deliver to customer':
                $scope.deliverToCustomerDetails = true;
                break;
            case 'deliver to employee':
                $scope.deliverToCustomerDetails = false;
                break;
                
        }
    };

    /*      map view page , second page for employee        */
    $scope.searchResults = function () {
        //continue with searching ! 
        var searchIn = $('#searchInput').val();
        var result = $scope.userEncryptors.includes(searchIn);
        Console.log(result);
    }

    $scope.initialize = function () {
        var googleMapOption = {
            zoom: 6.99,
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
     * also push markers to markesrArray */
    function initMarkers() {
        angular.forEach($scope.userEncryptors, function (encryptor, index) {
            var marker = new google.maps.Marker({
                //setting marker position
                position: new google.maps.LatLng(encryptor.deviceLocation.latitude, encryptor.deviceLocation.longitude),
                map: $scope.gMap
            });
            markesrArray.push(marker);
        });
    };

    /* introduction: clusterMarkers function cluster marker by their location 
       markerClusterer on google */
    function clusterMarkers() {
        var markerCluster = new MarkerClusterer($scope.gMap, markesrArray,
            { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
    }

});

/*
app.factory('dataSharing', function () {
    var data = {
        encryptorsArray: {}
    };
})*/