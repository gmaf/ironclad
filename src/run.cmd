docker-compose -f Docker\docker-compose.yml -f Docker\docker-compose.release.yml -p ironclad config
docker-compose -f Docker\docker-compose.yml -f Docker\docker-compose.release.yml -p ironclad build
docker rmi $(docker images -f "dangling=true" -q)
docker-compose -f Docker\docker-compose.yml -f Docker\docker-compose.release.yml -p ironclad up -d --force-recreate --remove-orphans