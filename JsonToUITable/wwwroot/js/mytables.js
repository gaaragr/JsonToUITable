//Hold the DataTable variable...
var mMyDataTable;
var mCalculatingJsonArrays = false;

$(document).ready(function () {
    console.log('My tables js loaded!');

    //Fix UI
    FixUIVisiblities();

    $('#rdGET,#rdPOST').click(function () {
        //Fix UI
        FixUIVisiblities();
    });

    $('#btnWsGo').click(function () {
        try {
            ExecuteRequestFromUI();
        }
        catch (e) {
            myAlert('An error occured: ' + e);
        }
    });

    $('#btnJsonGo').click(function () {
        try {
            FixDataTableFromJson();
        }
        catch (e) {
            myAlert('An error occured: ' + e);
        }
    });

    $('#href_tab_toogle').click(function () {
        $('.tab-content').toggle();
    });

    //JSON text area changed
    $('#txtJsonBody').bind('input propertychange', function () {
        HandleJSONTextChanged();
    });    
});

function FixUIVisiblities() {
    if ($('#rdGET').is(':checked')) { $('#groupPostData').hide(); };

    if ($('#rdPOST').is(':checked')) { $('#groupPostData').show(); };

    $('#lblJsonError').hide();
    $('#cont_selFields').hide();
}

function ExecuteRequestFromUI() {
    ShowLoading();
    var lHttpMethod = GetHttpMethodFromUI();
    var lUrl = $('#txtUrl').val();
    var lHeaders = $('#txtHeaders').val();
    var lData = $('#txtData').val();

    GetData(lUrl, lHttpMethod, lHeaders, lData);
}

function FixDataTableFromJson() {
    ShowLoading();
    var lJsontext = $('#txtJsonBody').val();
    var lJson = JSON.parse(lJsontext);
    CreateDataTable(lJson, GetSelectedArray());
}

function GetData(aCompleteUrl, aMethod, aHeaders, aData) {
    var lPostObj = { Url: aCompleteUrl, Method: '', Headers: aHeaders, PostData: aData };

    if (aMethod == 'GET') {
        lPostObj.Method = 0;
    }
    else {
        lPostObj.Method = 1;
    }
    var lPostDataStr = JSON.stringify(lPostObj);

    $.ajax({
        url: '/api/proxy',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: lPostDataStr,
        success: function (data, textStatus, jQxhr) {
            console.log('Got data from server: ' + data);
            HandleResponseFromServer(data);
        },
        error: function (jqXhr, textStatus, errorThrown) {
            myAlert('An error occured. Status code: ' + textStatus + ', error: ' + errorThrown + ", jqXHR: " + JSON.stringify(jqXhr));
        }
    });
}

function HandleResponseFromServer(aData)
{
    //Set to Json text area
    $('#txtJsonBody').val(JSON.stringify(aData));

    PopulateFieldsDropDownFromObject(aData);  

    //Go to JSON Tab
    $('#href_tab_json').click();    

    CreateDataTable(aData, GetSelectedArray());
}

function HandleJSONTextChanged()
{
    try {
        if (mCalculatingJsonArrays == false)
        {
            mCalculatingJsonArrays = true;
            var lJsontext = $('#txtJsonBody').val();
            var lJson = JSON.parse(lJsontext);

            $('#lblJsonError').hide();

            PopulateFieldsDropDownFromObject(lJson);
        }        
    }
    catch (e)
    {
        console.log('Error HandleJSONTextChanged: ' + e)
        $('#lblJsonError').show();
    }
    mCalculatingJsonArrays = false;
}

function PopulateFieldsDropDownFromObject(aObj)
{
    //clear select
    $('#selFields').empty();   
    $('#cont_selFields').hide();
    
   
    for (var propertyName in aObj) {
        // propertyName is what you want
        // you can get the value like this: myObject[propertyName]

        if ($.isArray(aObj[propertyName])) {
            $('#selFields').append(new Option(propertyName, propertyName));
            $('#cont_selFields').show();
        }
    }    
}

function GetSelectedArray()
{
    var lExit = $("#selFields").val();
    return lExit;
}

function GetHttpMethodFromUI() {
    var lExit = 'GET';

    if ($('#rdGET').is(':checked')) { lExit = 'GET'; };

    if ($('#rdPOST').is(':checked')) { lExit = 'POST'; };

    return lExit;
}

function CreateDataTable(aData, aPropertyName) {

    try {
        if (aPropertyName != null) {
            //If property is set, use that as the data object
            aData = aData[aPropertyName];
        }

        var lDtData = GetColumnsAndDataFromDataSet(aData);

        ReInitializeMyDataTable();

        HideLoading();


        mMyDataTable = $('#myDataTable').DataTable({
            data: lDtData[1],
            columns: lDtData[0]
        });
    }
    catch (e)
    {
        ReInitializeMyDataTable();

        console.log('DataTable failed, details: ' + e);

        myAlert('Failed to populate the UI table with the input data, please check your data...');
    }    
}

function ReInitializeMyDataTable()
{
    if (mMyDataTable != null) {
        mMyDataTable.destroy();
        $('#myDataTable').empty();
        mMyDataTable = null;
    }
}

function GetColumnsAndDataFromDataSet(aData) {
    var lExit = [];
    var lColumns = [];
    var lRows = [];

    //Add to returning obj
    lExit.push(lColumns);
    lExit.push(lRows);

    var lIncomingData = [];

    if ($.isArray(aData)) {
        lIncomingData = aData;
    }
    else {
        lIncomingData.push(aData);
    }

    //Iterate rows
    for (var i = 0; i < lIncomingData.length; i++) {
        var lRow = lIncomingData[i];
        var lNewRow = [];
        lRows.push(lNewRow);
        //Iterate Columns  
        for (var propertyName in lRow) {
            // propertyName is what you want
            // you can get the value like this: myObject[propertyName]
            if (i == 0) {
                lColumns.push({ "title": propertyName });
            }

            lNewRow.push(lRow[propertyName]);
        }
    }

    return lExit;
}

function ShowLoading() {
    $('#groupMyDataTable').hide();
}

function HideLoading() {
    $('#groupMyDataTable').show();
}

function myAlert(aMsg) {
    //TODO: make this better
    alert(aMsg);
}