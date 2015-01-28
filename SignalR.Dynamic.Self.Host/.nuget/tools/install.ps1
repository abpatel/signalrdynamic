param($installPath, $toolsPath, $package, $project)
$array = @(
$project.ProjectItems.Item("CSS").ProjectItems.Item("styles.css"),
$project.ProjectItems.Item("Scripts").ProjectItems.Item("jquery-2.1.3.min.js"),
$project.ProjectItems.Item("Views").ProjectItems.Item("Home").ProjectItems.Item("CreateOrEdit.cshtml"),
$project.ProjectItems.Item("Views").ProjectItems.Item("Home").ProjectItems.Item("Index.cshtml")
)

foreach($element in $array){
	#set 'Copy to Output Directory' to 'Copy if newer'
	$copyToOutput = $element.Properties.Item("CopyToOutputDirectory")
	$copyToOutput.Value = 1

	#set 'Build Action' to 'Content'
	$buildAction = $element.Properties.Item("BuildAction")
	$buildAction.Value = 2
}