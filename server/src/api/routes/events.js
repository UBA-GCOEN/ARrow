import express from 'express';
import session from '../middlewares/session.js'
import { csrfProtect } from '../middlewares/csrfProtection.js';
import authUser from '../middlewares/authUser.js'
import { deleteEvent, 
    getSpecificEvent, 
    getAllEvents, 
    updateEvent,
    createEvent 
} from '../controllers/events.js';


const router = express.Router();

router.post("/createEvent", session, authUser, createEvent)
router.put("/updateEvent", session, authUser, updateEvent)
router.get("/getAllEvents", getAllEvents)
router.get("/getSpecificEvent", getSpecificEvent)
router.delete("/deleteEvent", session, authUser, deleteEvent)

export default router;