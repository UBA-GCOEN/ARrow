import express from 'express';
import session from '../middlewares/session.js';
const router = express.Router();
import indexController from "../controllers/index.js";
import { logout } from '../middlewares/logout.js';
import { csrfProtect } from '../middlewares/csrfProtection.js';


router.get("/", session, csrfProtect, indexController)
router.get("/logout", session, csrfProtect, logout, indexController)

export default router;