API Reference:
--------------

api/workflows GET - get a list of available workflows
api/workflows POST - add new workflow
api/workflows/last-used GET - get the most recently modified workflow

api/workflows/{workflowId}/commits GET - get the commit history of the specified workflow
api/workflows/{workflowId}/commits POST - add new commit (save state)
api/workflows/{workflowId}/commits/{commitId} GET - get the specified commit (load state)
api/workflows/{workflowId}/commits/latest GET - get the latest commit

api/database/commits GET - get commit history of the database scheme
api/database/commits POST - add new commit (save state)
api/database/commits/{commitId} GET - get the specified commit (load state)
api/database/commits/latest GET - get the latest commit
