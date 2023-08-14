import express from 'express';
const router = express.Router();

import {userAdmin, signup, signin} from "../controllers/userAdmin.js";


router.get("/", userAdmin)
router.post("/signup", signup)
router.post("/signin", signin)

export default router;