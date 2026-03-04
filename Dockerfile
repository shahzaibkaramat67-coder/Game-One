FROM node:20-alpine

WORKDIR /app

# Install production deps first for better layer caching
COPY server/package.json server/package-lock.json ./
RUN npm ci --omit=dev

# Copy app source
COPY server/src ./src
COPY server/test-client.js ./test-client.js

ENV NODE_ENV=production
# Railway commonly sets PORT=8080; EXPOSE is metadata but some platforms infer routing from it.
EXPOSE 3000 8080

CMD ["node", "src/server.js"]
