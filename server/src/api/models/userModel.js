import mongoose from "mongoose";
import sanitizerPlugin from 'mongoose-sanitizer'

// User schema
const userModel = mongoose.Schema({
     image: { type: Buffer },
     id: { type: String },
     googleId: { type: String },
     name: { type: String },
     collegeEmail: { type: String },
     email: { type: String, required: true },
     password: { type: String },
     role: { type: String },
     branch: { type: String },
     subjects: { type: String },
     designation: { type: String },
     education: { type: String },
     year: { type: String },
     bio: { type: String },
     enrollNo: { type: Number },
     intrest: { type: String },
     mobile: { type: Number },
     isOnboarded: { type:Boolean, default:false},
})


// sanitize schema
userModel.plugin(sanitizerPlugin);


export default mongoose.model("users",userModel);