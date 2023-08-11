import express from 'express';
const router = express.Router();

import indexController from "../controllers/index.js";


router.get("/", indexController)


export default router;