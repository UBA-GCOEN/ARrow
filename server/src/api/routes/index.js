import express from 'express';
import session from '../middlewares/session.js';
const router = express.Router();

import indexController from "../controllers/index.js";


router.get("/", session, indexController)

export default router;