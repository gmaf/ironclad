docker-compose -f docker-compose.yml -f docker-compose.release.yml -p ironclad kill
docker-compose -f docker-compose.yml -f docker-compose.release.yml -p ironclad down --rmi local --remove-orphans