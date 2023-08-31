import mongoose from "mongoose";


//estimated student schema
const userFacultyModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     branch: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     subjects: { type: String, required: true },
     designation: { type: String, required: true },
     education: { type: String, required: true },
     bio: { type: String },
     intrest: { type: String }

})

export default mongoose.model("userFaculties",userFacultyModel);