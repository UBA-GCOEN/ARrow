import express from 'express';
import session from '../middlewares/session.js'

const router = express.Router();

import {userAdmin, signup, signin} from "../controllers/userAdmin.js";


router.get("/", userAdmin)
router.post("/signup", signup)
router.post("/signin", session, signin)

export default router;