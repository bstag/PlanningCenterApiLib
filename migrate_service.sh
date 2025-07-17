#!/bin/bash

# Script to migrate a service to ServiceBase pattern
# Usage: ./migrate_service.sh ServiceName

SERVICE_NAME=$1
SERVICE_FILE="src/PlanningCenter.Api.Client/Services/${SERVICE_NAME}.cs"

if [ ! -f "$SERVICE_FILE" ]; then
    echo "Service file not found: $SERVICE_FILE"
    exit 1
fi

echo "Migrating $SERVICE_NAME to ServiceBase pattern..."

# Step 1: Update class declaration to inherit from ServiceBase
sed -i "s/public class ${SERVICE_NAME} : I${SERVICE_NAME}/public class ${SERVICE_NAME} : ServiceBase, I${SERVICE_NAME}/" "$SERVICE_FILE"

# Step 2: Remove private fields and update constructor
sed -i '/private readonly IApiConnection _apiConnection;/d' "$SERVICE_FILE"
sed -i '/private readonly ILogger<.*> _logger;/d' "$SERVICE_FILE"

# Step 3: Replace field references
sed -i 's/_logger/Logger/g' "$SERVICE_FILE"
sed -i 's/_apiConnection/ApiConnection/g' "$SERVICE_FILE"

# Step 4: Update constructor to call base constructor
sed -i 's/public '"${SERVICE_NAME}"'(IApiConnection apiConnection, ILogger<'"${SERVICE_NAME}"'> logger)/public '"${SERVICE_NAME}"'(IApiConnection apiConnection, ILogger<'"${SERVICE_NAME}"'> logger)\n        : base(logger, apiConnection)/' "$SERVICE_FILE"

# Step 5: Remove constructor body null checks
sed -i '/apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));/d' "$SERVICE_FILE"
sed -i '/logger ?? throw new ArgumentNullException(nameof(logger));/d' "$SERVICE_FILE"

echo "Migration completed for $SERVICE_NAME"
echo "Manual verification recommended - some methods may need ExecuteAsync wrapping"