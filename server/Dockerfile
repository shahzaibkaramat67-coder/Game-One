FROM node:20-alpine

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci --omit=dev

COPY src ./src
COPY test-client.js ./test-client.js

ENV NODE_ENV=production
# Keep both common ports exposed; app uses $PORT at runtime.
EXPOSE 3000 8080

CMD ["node", "src/server.js"]
