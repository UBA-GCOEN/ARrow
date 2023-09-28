import mongoose from "mongoose";

const eventModel = new mongoose.Schema({
    // image: { type: Buffer },
    title: { type: String, required: true},
    description: { type:String, required:true},
    organizerRole: { type:String, required:true},
    organizerEmail: { type:String, required:true},
    organizerName: { type:String, required:true},
    status: { type:String, required:true, default:'upcoming'},  //ongoing, past, upcoming
    eventCoordinator: { type:String },
    time: { type:String },
    venue: { type:String },
    guest: { type:String }
})

export default mongoose.model("events", eventModel);