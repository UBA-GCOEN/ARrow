import express from 'express';
const router = express.Router();

import {userAdmin, signup} from "../controllers/userAdmin.js";


router.get("/", userAdmin)
router.post("/signup", signup)

export default router;