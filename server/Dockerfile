FROM node:18

WORKDIR /usr/src/app

# Install app dependencies
# A wildcard is used to ensure both package.json AND package-lock.json are copied
# where available (npm@5+)
COPY package*.json ./

RUN npm install --omit=dev
# If you are building your code for development
# RUN npm install


COPY . .

EXPOSE 5000

CMD [ "npm", "run", "server" ]