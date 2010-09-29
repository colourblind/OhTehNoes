Oh Teh Noes is a .NET console application designed to alert the user to 
potential problems on their boxen and is intended to be run as a scheduled 
task.

USAGE

OhTehNoes [taskFile]

taskFile    The tasks to run. If this is not supplied, OhTehNoes will try 
            to use tasks.xml from the current working directory.

PLUGINS

All of the operations that Oh Teh Noes can perform (even the core 
functionality) it loads from plugins. When run, OhTehNoes will scan the 
/plugins/ directory and attempt to load tasks from the assemblies in there.

TASK FILES

The task file is an XML file with the following format:

<tasks logName="LOG_NAME">
    <task type="TASK_TYPE" 
        name="TASK_NAME"
        taskSpecificAttribute1="foo" 
        taskSpecificAttribute2="bar" />
</tasks>

Except for the required 'type' attribute and the optional 'name' attribute, 
the attributes of each task element vary depending on the task itself.

Check the /resources/task.xml file for a working example containing the core
plugins.
