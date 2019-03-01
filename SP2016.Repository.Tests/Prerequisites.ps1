param(
    [string]$WebUrl
)
 
function CreateList($listName) {  
    $spWeb = Get-SPWeb -Identity $WebUrl
    $spTemplate = $spWeb.ListTemplates["Custom List"]
    $spListCollection = $spWeb.Lists
    $spListCollection.Add($listName, $listName, $spTemplate) 
}

function CreateDepartmentsList() {
	$spWeb = Get-SPWeb -Identity $WebUrl
	$listName = "Departments"
    CreateList $listName
    $departmentsList = $spWeb.Lists[$listName]

	#Dismissed Field
    $employeesCountFieldName = $departmentsList.Fields.Add("EmployeesCount", [Microsoft.SharePoint.SPFieldType]::Number, $false)
	$employeesCountField = [Microsoft.SharePoint.SPFieldNumber]$departmentsList.Fields[$employeesCountFieldName]
	$employeesCountField.DisplayFormat = [Microsoft.SharePoint.SPNumberFormatTypes]::NoDecimal
	$employeesCountField.Update()

	#Update List
    $departmentsList.Update()  
}

function CreateUsersList() {
	$spWeb = Get-SPWeb -Identity $WebUrl
	$listName = "Users"
    CreateList $listName
    $usersList = $spWeb.Lists[$listName]

	#Dismissed Field
    $usersList.Fields.Add("Dismissed", [Microsoft.SharePoint.SPFieldType]::Boolean, $false)

	#Description Field
	$newFieldName = $usersList.Fields.Add("Description", [Microsoft.SharePoint.SPFieldType]::Note, $false)
    $newField = $usersList.Fields.GetFieldByInternalName($newFieldName)
    $newField.RichText = $true
    $newField.RichTextMode = "FullHtml"
    $newField.Update()

	#Skills Field
	$newFieldName = $usersList.Fields.Add("Skills", [Microsoft.SharePoint.SPFieldType]::Note, $false)
    $newField = $usersList.Fields.GetFieldByInternalName($newFieldName)
    $newField.RichText = $false
    $newField.Update()

	#BirthDate Field
	$usersList.Fields.Add("BirthDate", [Microsoft.SharePoint.SPFieldType]::DateTime, $false)

	#Department Field
	$lookupList = $spWeb.Lists["Departments"] 
	$usersList.Fields.AddLookup("Department", $lookupList.ID, $false)
	$departmentsLookupField = $lookupList.Fields["Title"]
	$departmentsLookupField.Update()

	#Link Field
	$usersList.Fields.Add("Link", [Microsoft.SharePoint.SPFieldType]::URL, $false)

	#JobTitle Field
	$usersList.Fields.Add("JobTitle", [Microsoft.SharePoint.SPFieldType]::Text, $false)

	#Seniority Field
	$seniorityChoices = New-Object System.Collections.Specialized.StringCollection;
	$seniorityChoices.Add("Junior Developer");
	$seniorityChoices.Add("Middle Developer");
	$seniorityChoices.Add("Senior Developer");
	$usersList.Fields.Add("Seniority", [Microsoft.SharePoint.SPFieldType]::Choice, $false, $false, $seniorityChoices)

	#Update List
    $usersList.Update()  
}

function CreateRoomsList() {
	$spWeb = Get-SPWeb -Identity $WebUrl
	$listName = "Rooms"
    CreateList $listName
    $usersList = $spWeb.Lists[$listName]
}

CreateDepartmentsList
CreateUsersList
CreateRoomsList