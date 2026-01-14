// https://angular.io/guide/build#proxying-to-a-backend-server

const PROXY_CONFIG = {
  '/api': {
    target: 'https://localhost:7063',
    changeOrigin: true,
    secure: false,
    logLevel: 'debug',
    pathRewrite: {
      '^/api': '/api'
    },
    onProxyReq: (proxyReq, req, res) => {
      console.log(`[PROXY] ${req.method} ${req.url} -> ${proxyReq.protocol}//${proxyReq.host}${proxyReq.path}`);
      // Adicionar headers necessários
      proxyReq.setHeader('Origin', 'https://localhost:7063');
    },
    onProxyRes: (proxyRes, req, res) => {
      console.log(`[PROXY] ${req.method} ${req.url} -> ${proxyRes.statusCode}`);
      // Adicionar headers CORS na resposta
      proxyRes.headers['Access-Control-Allow-Origin'] = '*';
      proxyRes.headers['Access-Control-Allow-Methods'] = 'GET, POST, PUT, DELETE, OPTIONS';
      proxyRes.headers['Access-Control-Allow-Headers'] = 'Origin, X-Requested-With, Content-Type, Accept, Authorization';
    },
    onError: (err, req, res) => {
      console.error('[PROXY ERROR]', err.message);
      console.error(`[PROXY ERROR] Request: ${req.method} ${req.url}`);
      console.error(`[PROXY ERROR] Target: https://localhost:7063${req.url}`);
    },
    bypass: function (req, res, proxyOptions) {
      // Log todas as requisições que passam pelo proxy
      console.log(`[PROXY BYPASS CHECK] ${req.method} ${req.url}`);
      // Não fazer bypass, deixar o proxy processar
      return null;
    }
  }
};

module.exports = PROXY_CONFIG;
